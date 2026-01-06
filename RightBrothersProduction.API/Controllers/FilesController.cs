using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RightBrothersProduction.API.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.StaticFiles; // For ContentType
    using RightBrothersProduction.DataAccess.Repositories.IRepositories;
    using RightBrothersProduction.Models;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FilesController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        // Define limits
        private const long MaxProfilePicSize = 2 * 1024 * 1024; // 2 MB
        private readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };

        public FilesController(
            IWebHostEnvironment env,
            IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> userManager)
        {
            _env = env;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        // =================================================================
        // 1. SECURE PROXY DOWNLOAD (For Request Attachments)
        // =================================================================
        // Usage: <a href="api/files/requests/download/105/error.log">File</a>
        [HttpGet("requests/download/{requestId}/{fileName}")]
        public async Task<IActionResult> DownloadRequestFile(int requestId, string fileName)
        {
            var request = await _unitOfWork.Request.Get(r => r.Id == requestId);
            if (request == null) return NotFound("Request not found.");

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (request.CreatedById != currentUserId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var folderPath = Path.Combine(_env.ContentRootPath, "uploads", "requests", requestId.ToString());
            var filePath = Path.Combine(folderPath, fileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound("File not found on server.");

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(fileName, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(bytes, contentType, fileName);
        }

        // =================================================================
        // 2. PROFILE PICTURE MANAGEMENT
        // =================================================================

        // POST: api/files/profile-picture
        [HttpPost("profile-picture")]
        public async Task<IActionResult> UploadProfilePicture(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            if (file.Length > MaxProfilePicSize)
                return BadRequest("Image size exceeds the 2MB limit.");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
                return BadRequest("Invalid image format. Allowed: JPG, PNG, WEBP.");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return Unauthorized();


            string webRootPath = _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");
            string uploadsFolder = Path.Combine(webRootPath, "uploads", "profiles");

            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);


            if (!string.IsNullOrEmpty(user.ProfilePictureUrl))
            {
                var oldPath = Path.Combine(uploadsFolder, user.ProfilePictureUrl);
                if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
            }

            string uniqueFileName = $"{userId}_{Guid.NewGuid()}{extension}";
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            user.ProfilePictureUrl = uniqueFileName;
            await _userManager.UpdateAsync(user);

            return Ok(new
            {
                message = "Profile picture updated",
                profilePictureUrl = uniqueFileName
            });
        }

        [HttpDelete("profile-picture")]
        public async Task<IActionResult> RemoveProfilePicture()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return Unauthorized();

            if (string.IsNullOrEmpty(user.ProfilePictureUrl))
                return BadRequest("No profile picture to delete.");

            string webRootPath = _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");
            string uploadsFolder = Path.Combine(webRootPath, "uploads", "profiles");
            var filePath = Path.Combine(uploadsFolder, user.ProfilePictureUrl);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            user.ProfilePictureUrl = null;
            await _userManager.UpdateAsync(user);

            return Ok(new { message = "Profile picture removed" });
        }
    }
}
