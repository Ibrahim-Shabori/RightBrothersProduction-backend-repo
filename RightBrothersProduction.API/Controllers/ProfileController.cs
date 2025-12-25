
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RightBrothersProduction.API.DTOs;
using RightBrothersProduction.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using static RightBrothersProduction.Models.RequestModels;

namespace RightBrothersProduction.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public ProfileController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userProfile = await _userManager.Users
                .Where(u => u.Id == userId).Include("RequestsCreated")
                .Select(u => new UserProfileDto
                {
                    FullName = u.FullName,
                    TotalRequests = u.RequestsCreated.Count(),
                    TotalVotesReceived = u.RequestsCreated.Sum(r => r.VotesCount),
                    ImplementedRequests = u.RequestsCreated.Count(r => r.Status == RequestStatus.Done),
                    Bio = u.Bio,
                    ProfilePictureUrl = u.ProfilePictureUrl,
                    DateJoined = u.DateJoined != null ? u.DateJoined : DateTime.MinValue,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                }).FirstOrDefaultAsync();
            if (userProfile == null) {
                return NotFound("Profile Not Found");
            }
            return Ok(userProfile);
        }

        [HttpGet("contact")]
        public async Task<IActionResult> GetUserContactInfo()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userContactInfo = await _userManager.Users
                .Where(u => u.Id == userId)
                .Select(u => new UserContactInfoDto
                {
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                }).FirstOrDefaultAsync();
            if (userContactInfo == null) {
                return NotFound("Contact Info Not Found");
            }
            return Ok(userContactInfo);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserProfileForAViewer(string userId)
        {
            var userProfile = await _userManager.Users
                .Where(u => u.Id == userId).Include("RequestsCreated")
                .Select(u => new UserProfileForAViewerDto
                {
                    FullName = u.FullName,
                    TotalRequests = u.RequestsCreated.Count(),
                    TotalVotesReceived = u.RequestsCreated.Sum(r => r.VotesCount),
                    ImplementedRequests = u.RequestsCreated.Count(r => r.Status == RequestStatus.Done),
                    Bio = u.Bio,
                    ProfilePictureUrl = u.ProfilePictureUrl,
                    DateJoined = u.DateJoined != null ? u.DateJoined : DateTime.MinValue,
                }).FirstOrDefaultAsync();

            if (userProfile == null) {
                return NotFound("Profile Not Found");
            }

            return Ok(userProfile);
        }

        [HttpGet("basic/{userId}")]
        public async Task<IActionResult> GetBasicUserProfile(string userId) { 
            var userProfile = await _userManager.Users
                .Where(u => u.Id == userId)
                .Select(u => new BasicUserProfileDto
                {
                    FullName = u.FullName,
                    ProfilePictureUrl = u.ProfilePictureUrl,
                }).FirstOrDefaultAsync();
            if (userProfile == null) {
                return NotFound("Profile Not Found");
            }
            return Ok(userProfile);
        }

        [HttpGet("basic")]
        public async Task<IActionResult> GetBasicAllUserProfiles() {
            var userProfiles = await _userManager.Users
                .Select(u => new BasicUserProfileDto
                {
                    FullName = u.FullName,
                    ProfilePictureUrl = u.ProfilePictureUrl,
                }).ToListAsync();
            if (userProfiles == null || userProfiles.Count == 0) {
                return NotFound("No Profiles Found");
            }
            return Ok(userProfiles);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfileDto dto)
        {
            // 1. Get Current User
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null) return NotFound();

            // 2. Update Basic Info (Always safe)
            user.FullName = dto.FullName;
            user.PhoneNumber = dto.PhoneNumber;
            user.Bio = dto.Bio;

            // 3. Handle Password Change (Only if requested)
            if (!string.IsNullOrEmpty(dto.NewPassword))
            {
                // A. Validate they provided the current password
                if (string.IsNullOrEmpty(dto.CurrentPassword))
                {
                    return BadRequest("To change your password, you must verify your current password.");
                }

                // B. Attempt the change using Identity Manager
                var changeResult = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);

                if (!changeResult.Succeeded)
                {
                    return BadRequest(changeResult.Errors);
                }
            }

            // 4. Save Changes
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
                return Ok(new { message = "Profile updated successfully" });

            return BadRequest(result.Errors);
        }
    }
}
