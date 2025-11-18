using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RightBrothersProduction.API.DTOs;
using RightBrothersProduction.DataAccess.Repositories.IRepositories;
using RightBrothersProduction.Models;
using System.Security.Claims;
using static RightBrothersProduction.Models.RequestModels;

namespace RightBrothersProduction.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class RequestController : ControllerBase
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _env;
        public RequestController(IUnitOfWork unitOfWork, IWebHostEnvironment env)
        {
            _unitOfWork = unitOfWork;
            _env = env;
        }

        [HttpGet]
        public IActionResult GetRequests()
        {
            var requests = _unitOfWork.Request.GetAll(IncludeProperties: "Files,CreatedBy");

            var dtoList = requests.Select(r => new
            {
                r.Id,
                r.Title,
                r.Description,
                r.Status,
                r.CreatedAt,
                r.VotesCount,
                r.Type,
                r.CategoryId,
                r.CreatedBy.FullName,
                Files = r.Files.Select(f => new
                {
                    f.Id,
                    f.FileName,
                    f.ContentType,
                    f.Size
                }).ToList()
            });


            return Ok(dtoList);
        }

        [HttpGet("bugs")]
        public IActionResult GetBugs()
        {
            var bugs = _unitOfWork.Request.GetAll(r => r.Type == RequestType.Bug || r.Type == RequestType.DetailedBug, "Files,CreatedBy");
            var dtoList = bugs.Select(r => new
            {
                r.Id,
                r.Title,
                r.Description,
                r.Status,
                r.CreatedAt,
                r.VotesCount,
                r.Type,
                r.CategoryId,
                r.CreatedBy.FullName,
                Files = r.Files.Select(f => new
                {
                    f.Id,
                    f.FileName,
                    f.ContentType,
                    f.Size
                }).ToList()
            });
            return Ok(dtoList);
        }

        [HttpGet("features")]
        public IActionResult GetFeatures()
        {
            var features = _unitOfWork.Request.GetAll(r => (r.Type == RequestType.Regular || r.Type == RequestType.Detailed), "Files,CreatedBy");
            var dtoList = features.Select(r => new
            {
                r.Id,
                r.Title,
                r.Description,
                r.Status,
                r.CreatedAt,
                r.VotesCount,
                r.Type,
                r.CategoryId,
                r.CreatedBy.FullName,
                Files = r.Files.Select(f => new
                {
                    f.Id,
                    f.FileName,
                    f.ContentType,
                    f.Size
                }).ToList()
            });
            return Ok(dtoList);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var request = _unitOfWork.Request.Get(r => r.Id == id, IncludeProperties: "Files,CreatedBy");

            if (request == null)
                return NotFound();

            var dto = new
            {
                request.Id,
                request.Title,
                request.Description,
                request.Status,
                request.CreatedAt,
                request.VotesCount,
                request.Type,
                request.CategoryId,
                request.CreatedBy.FullName,
                Files = request.Files.Select(f => new
                {
                    f.Id,
                    f.FileName,
                    f.ContentType,
                    f.Size
                }).ToList()
            };

            return Ok(request);
        }

        [HttpPost]
        public IActionResult Create([FromForm] CreateRequestDto dto)
        {
            //var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //if (userId == null)
            //    return Unauthorized();

            var request = new Request
            {
                Title = dto.Title,
                Description = dto.Description,
                CategoryId = dto.CategoryId,
                Type = dto.Type,
                CreatedAt = DateTime.UtcNow,
                Status = RequestStatus.UnderReview,
                VotesCount = 0,
                CreatedById = "1e5fc32a-f11d-4517-8eae-be9e39d285b5" // use auth later
            };

            _unitOfWork.Request.Add(request);
            _unitOfWork.Save();

            // Handle file upload
            if (dto.Attachments != null && dto.Attachments.Count > 0)
            {
                string uploadPath = Path.Combine(_env.ContentRootPath, "uploads", "requests", request.Id.ToString());

                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                foreach (var file in dto.Attachments)
                {
                    string filePath = Path.Combine(uploadPath, file.FileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    var fileRecord = new RequestFile
                    {
                        RequestId = request.Id,
                        FileName = file.FileName,
                        ContentType = file.ContentType,
                        Size = file.Length
                    };

                    _unitOfWork.RequestFile.Add(fileRecord);
                }

                _unitOfWork.Save();
            }

            return Ok(new { message = "Request created", id = request.Id });
        }

    }

}
