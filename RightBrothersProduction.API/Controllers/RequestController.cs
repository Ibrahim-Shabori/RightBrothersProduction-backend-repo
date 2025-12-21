using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RightBrothersProduction.API.DTOs;
using RightBrothersProduction.DataAccess.Repositories;
using RightBrothersProduction.DataAccess.Repositories.IRepositories;
using RightBrothersProduction.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using static RightBrothersProduction.Models.RequestModels;
using Request = RightBrothersProduction.Models.Request;

namespace RightBrothersProduction.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RequestController : ControllerBase
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _env;
        private const long MaxTotalSize = 20 * 1024 * 1024; // 20 MB
        public RequestController(IUnitOfWork unitOfWork, IWebHostEnvironment env)
        {
            _unitOfWork = unitOfWork;
            _env = env;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetRequests()
        {
            var requests = await _unitOfWork.Request.GetAll(IncludeProperties: "CreatedBy,Category");

            var dtoList = requests.Select(r => new
            {
                r.Id,
                r.Title,
                r.Description,
                r.Status,
                r.CreatedAt,
                r.VotesCount,
                r.Type,

                CategoryName = r.Category.Name,

                CreatedByName = r.CreatedBy.FullName,
                CreatedById = r.CreatedById,

                IsDetailed = (r.Type == RequestType.Detailed || r.Type == RequestType.DetailedBug)
            });

            return Ok(dtoList);
        }

        [HttpGet("bugs")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBugs()
        {
            var bugs = await _unitOfWork.Request.GetAll(r => r.Type == RequestType.Bug || r.Type == RequestType.DetailedBug, "CreatedBy,Category");
            var dtoList = bugs.Select(r => new
            {
                r.Id,
                r.Title,
                r.Description,
                r.Status,
                r.CreatedAt,
                r.VotesCount,
                r.Type,

                CategoryName = r.Category.Name,

                CreatedByName = r.CreatedBy.FullName,
                CreatedById = r.CreatedById,

                IsDetailed = (r.Type == RequestType.DetailedBug)
            });
            return Ok(dtoList);
        }

        [HttpGet("features")]
        [AllowAnonymous]
        public async Task<IActionResult> GetFeatures()
        {
            var features = await _unitOfWork.Request.GetAll(r => (r.Type == RequestType.Regular || r.Type == RequestType.Detailed), "CreatedBy,Category");
            var dtoList = features.Select(r => new
            {
                r.Id,
                r.Title,
                r.Description,
                r.Status,
                r.CreatedAt,
                r.VotesCount,
                r.Type,

                CategoryName = r.Category.Name,

                CreatedByName = r.CreatedBy.FullName,
                CreatedById = r.CreatedById,

                IsDetailed = (r.Type == RequestType.Detailed)
            });
            return Ok(dtoList);
        }

        [HttpGet("features/regular")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRegularFeatures()
        {
            var features = await _unitOfWork.Request.GetAll(r => r.Type == RequestType.Regular, "Files,CreatedBy");
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

        [HttpGet("features/detailed")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDetailedFeatures()
        {
            var features = await _unitOfWork.Request.GetAll(r => r.Type == RequestType.Detailed, "Files,CreatedBy");
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

        [HttpGet("bugs/regular")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRegularBugs()
        {
            var bugs = await _unitOfWork.Request.GetAll(r => r.Type == RequestType.Bug, "Files,CreatedBy");
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

        [HttpGet("bugs/detailed")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDetailedBugs()
        {
            var bugs = await _unitOfWork.Request.GetAll(r => r.Type == RequestType.DetailedBug, "Files,CreatedBy");
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

        [HttpGet("{id}")]
        [AllowAnonymous]

        public async Task<IActionResult> Get(int id)
        {
            var request = await _unitOfWork.Request.Get(r => r.Id == id, IncludeProperties: "Files,CreatedBy,DetailedRequest");

            if (request == null)
                return NotFound();

            if (request.DetailedRequest != null)
                {
                    var dtoDtailed = new {
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
                        }).ToList(),
                        request.DetailedRequest.DetailedDescription,
                        request.DetailedRequest.UsageDurationInMonths,
                        request.DetailedRequest.UrgencyCause,
                        request.DetailedRequest.AdditionalNotes,
                        request.DetailedRequest.ContributerPhoneNumber,
                        request.DetailedRequest.ContributerEmail
                    };

                    return Ok(dtoDtailed);
            }

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

            return Ok(dto);
        }

        [HttpGet("user/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRequestsByUser(string id)
        {
            var requests = await _unitOfWork.Request.GetAll(r => r.CreatedById == id, IncludeProperties: "CreatedBy,Category");
            var dtoList = requests.Select(r => new
            {
                r.Id,
                r.Title,
                r.Description,
                r.Status,
                r.CreatedAt,
                r.VotesCount,
                r.Type,
                CategoryName = r.Category.Name,
                CreatedByName = r.CreatedBy.FullName,
                });
            return Ok(dtoList);
        }

        [HttpGet("votes/{id}")]
        [AllowAnonymous]
        public IActionResult GetWithVoters(int id)
        {
            var request = _unitOfWork.Request.GetRequestWithVoters(id);
            if (request == null)
                return NotFound();

            if (request.DetailedRequest != null)
            {
                var dtoDtailed = new
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
                    }).ToList(),
                    request.DetailedRequest.DetailedDescription,
                    request.DetailedRequest.UsageDurationInMonths,
                    request.DetailedRequest.UrgencyCause,
                    request.DetailedRequest.AdditionalNotes,
                    request.DetailedRequest.ContributerPhoneNumber,
                    request.DetailedRequest.ContributerEmail,
                    Voters = request.Votes.Select(v => new
                    {
                        v.User.FullName,
                        v.VotedAt,
                    }).ToList()
                };

                return Ok(dtoDtailed);
            }

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
                }).ToList(),
                Voters = request.Votes.Select(v => new
                {
                    v.User.FullName,
                    v.VotedAt,
                }).ToList()
            };

            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateRequestDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();
            

            var request = new Request
            {
                Title = dto.Title,
                Description = dto.Description,
                CategoryId = dto.CategoryId,
                Type = dto.Type,
                CreatedAt = DateTime.UtcNow,
                Status = RequestStatus.UnderReview,
                VotesCount = 0,
                IsRegistered = false,
                CreatedById = userId
            };

            await _unitOfWork.Request.Add(request);
            await _unitOfWork.Save();

            // Handle file upload
            if (dto.Attachments != null && dto.Attachments.Count > 0)
            {
                await SaveRequestAttachments(request.Id, dto.Attachments);

                await _unitOfWork.Save();
            }

            return Ok(new { message = "Request created", id = request.Id });
        }
        [HttpPost("detailed")]
        public async Task<IActionResult> CreateDetailedRequest([FromForm] CreateDetailedRequestDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var request = new Request
            {
                Title = dto.Title,
                Description = dto.Description,
                CategoryId = dto.CategoryId,
                Type = dto.Type,
                CreatedAt = DateTime.UtcNow,
                Status = RequestStatus.UnderReview,
                VotesCount = 0,
                CreatedById = userId
            };
            await _unitOfWork.Request.Add(request);
            await _unitOfWork.Save();
            // Handle file upload
            if (dto.Attachments != null && dto.Attachments.Count > 0)
            {
                await SaveRequestAttachments(request.Id, dto.Attachments);
                await _unitOfWork.Save();

                var detailedRequest = new DetailedRequest
                {
                    RequestId = request.Id,
                    DetailedDescription = dto.DetailedDescription,
                    UsageDurationInMonths = dto.UsageDurationInMonths,
                    UrgencyCause = dto.UrgencyCause,
                    AdditionalNotes = dto.AdditionalNotes,
                    ContributerPhoneNumber = dto.ContributerPhoneNumber,
                    ContributerEmail = dto.ContributerEmail
                };
                await _unitOfWork.DetailedRequest.Add(detailedRequest);
                await _unitOfWork.Save();
            }
            return Ok(new { message = "Detailed request created", id = request.Id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateRequestDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // Fetch request + its existing files
            var request =  await _unitOfWork.Request.Get(r => r.Id == id, "Files", true);

            if (request == null)
                return NotFound();

            if (request.CreatedById != userId)
            {
                return Unauthorized();
            }
                // Step 1: Calculate size of existing files that user wants to keep
                long existingSize = request.Files
                .Where(f => dto.ExistingAttachmentIds.Contains(f.Id))
                .Sum(f => f.Size);

            // Step 2: Calculate size of new attachments being uploaded
            long newFilesSize = dto.NewAttachments.Sum(f => f.Length);

            long totalSize = existingSize + newFilesSize;

            // Step 3: Validate total size
            if (totalSize > MaxTotalSize)
            {
                return BadRequest($"Total file size cannot exceed {MaxTotalSize / (1024 * 1024)} MB.");
            }

            // Step 4: Remove deleted files
            var removedFiles = request.Files
                .Where(f => !dto.ExistingAttachmentIds.Contains(f.Id))
                .ToList();
            string uploadPath = Path.Combine(_env.ContentRootPath, "uploads", "requests", request.Id.ToString());
            string filePath;
            foreach (var file in removedFiles)
            {
                filePath = Path.Combine(uploadPath, file.FileName);
                System.IO.File.Delete(filePath);
                _unitOfWork.RequestFile.Remove(file);
            }

            // Step 5: Save new files to storage
            await SaveRequestAttachments(request.Id, dto.NewAttachments);

            // Step 6: Update request fields
            request.Title = dto.Title;
            request.Description = dto.Description;

            await _unitOfWork.Save();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue("role");
            var request = await _unitOfWork.Request.Get(r => r.Id == id, "Files");
            if (request == null)
                return NotFound();
            if (request.CreatedById != userId && userRole != "Admin")
            {
                return Unauthorized();
            }
            // Delete associated files from storage
            string uploadPath = Path.Combine(_env.ContentRootPath, "uploads", "requests", request.Id.ToString());
            if (Directory.Exists(uploadPath))
            {
                Directory.Delete(uploadPath, true);
            }
            // Delete associated file records
            foreach (var file in request.Files)
            {
                _unitOfWork.RequestFile.Remove(file);
            }
            // Delete the request
            _unitOfWork.Request.Remove(request);
            await _unitOfWork.Save();
            return NoContent();
        }

        private async Task SaveRequestAttachments(int requestId, List<IFormFile> attachments)
        {
            string uploadPath = Path.Combine(_env.ContentRootPath, "uploads", "requests", requestId.ToString());

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            foreach (var file in attachments)
            {
                // Optional: Generate safe filename here if you want
                string filePath = Path.Combine(uploadPath, file.FileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var fileRecord = new RequestFile
                {
                    RequestId = requestId,
                    FileName = file.FileName,
                    ContentType = file.ContentType,
                    Size = file.Length
                };

                await _unitOfWork.RequestFile.Add(fileRecord);
            }
        }

    }

}
