using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RightBrothersProduction.API.DTOs;
using RightBrothersProduction.API.Services.IServices;
using RightBrothersProduction.DataAccess.Repositories;
using RightBrothersProduction.DataAccess.Repositories.IRepositories;
using RightBrothersProduction.Models;
using RightBrothersProduction.Models.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using static RightBrothersProduction.Models.PeriodModels;
using static RightBrothersProduction.Models.RequestModels;
using Request = RightBrothersProduction.Models.Request;

namespace RightBrothersProduction.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RequestController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestService _requestService;
        private readonly IUserContext _userContext;
        private readonly IWebHostEnvironment _env;
        private const long MaxTotalSize = 20 * 1024 * 1024; // 20 MB
        public RequestController(IUnitOfWork unitOfWork, IUserContext userContext, UserManager<ApplicationUser> userManager, IRequestService requestService, IWebHostEnvironment env)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _requestService = requestService;
            _userContext = userContext;
            _env = env;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetRequests()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var requestsQuery = await _unitOfWork.Request.GetAllAsQueryable(IncludeProperties: "CreatedBy,Category");  

            var dtoListQuery = requestsQuery.Select(r => new
            {
                r.Id,
                r.Title,
                r.Description,
                r.Status,
                r.CreatedAt,
                r.CategoryId,
                r.VotesCount,
                r.Type,

                CategoryName = r.Category.Name,

                CreatedByName = r.CreatedBy.FullName,
                CreatedById = r.CreatedById,

                IsDetailed = (r.Type == RequestType.Detailed || r.Type == RequestType.DetailedBug),
                IsVotedByCurrentUser =  r.Votes.Any(v => v.UserId == userId)

            });

            var dtoList = await dtoListQuery.ToListAsync();

            return Ok(dtoList);
        }

        [HttpGet("bugs")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBugs()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var bugsQuery = await _unitOfWork.Request.GetAllAsQueryable(r => r.Type == RequestType.Bug || r.Type == RequestType.DetailedBug, "CreatedBy,Category");
            var dtoListQuery = bugsQuery.Select(r => new
            {
                r.Id,
                r.Title,
                r.Description,
                r.Status,
                r.CreatedAt,
                r.VotesCount,
                r.Type,
                r.CategoryId,

                CategoryName = r.Category.Name,

                CreatedByName = r.CreatedBy.FullName,
                CreatedById = r.CreatedById,

                IsDetailed = (r.Type == RequestType.DetailedBug),
                IsVotedByCurrentUser = r.Votes.Any(v => v.UserId == userId)
            });
            var dtoList = await dtoListQuery.ToListAsync();
            return Ok(dtoList);
        }

        [HttpGet("features")]
        [AllowAnonymous]
        public async Task<IActionResult> GetFeatures()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var featuresQuery = await _unitOfWork.Request.GetAllAsQueryable(r => (r.Type == RequestType.Regular || r.Type == RequestType.Detailed), "CreatedBy,Category");
            var dtoListQuery = featuresQuery.Select(r => new
            {
                r.Id,
                r.Title,
                r.Description,
                r.Status,
                r.CreatedAt,
                r.VotesCount,
                r.Type,
                r.CategoryId,

                CategoryName = r.Category.Name,

                CreatedByName = r.CreatedBy.FullName,
                CreatedById = r.CreatedById,

                IsDetailed = (r.Type == RequestType.Detailed),
                IsVotedByCurrentUser = r.Votes.Any(v => v.UserId == userId)
            });
            var dtoList = await dtoListQuery.ToListAsync();
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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var requestsQuery = await _unitOfWork.Request.GetAllAsQueryable(r => r.CreatedById == id, IncludeProperties: "CreatedBy,Category");
            var dtoListQuery = requestsQuery.Select(r => new
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
                IsVotedByCurrentUser = r.Votes.Any(v => v.UserId == userId)
            });
            var dtoList = await dtoListQuery.ToListAsync();
            return Ok(dtoList);
        }

        [HttpGet("voted")]
        public async Task<IActionResult> GetVotedRequests()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();
            var votedRequestsQuery = await _unitOfWork.Request.GetAllAsQueryable(r => r.Votes.Any(v => v.UserId == userId), IncludeProperties: "CreatedBy,Category");
            var dtoListQuery = votedRequestsQuery.Select(r => new
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
                IsVotedByCurrentUser = true,
                });
            var dtoList = await dtoListQuery.ToListAsync();
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

        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpGet("review/pages")]
        public async Task<ActionResult<IEnumerable<RequestPageItemDto>>> GetRequestsToReviewPage([FromQuery] RequestQueryParameters userParams)
        {
            var pagedList = await _requestService.GetRequestsAsync(userParams);


            // Create a metadata object
            var metadata = new
            {
                pagedList.TotalCount,
                pagedList.PageSize,
                pagedList.CurrentPage,
                pagedList.TotalPages,
                pagedList.HasNext,
                pagedList.HasPrevious
            };

            // Add to Headers (Angular can read this easily)
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(metadata));

            return Ok(pagedList); // Returns just the array of data
        }

        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpGet("manage/pages")]
        public async Task<ActionResult<IEnumerable<RequestManagementPageItemDto>>> GetRequestsToManage([FromQuery] RequestManagementQueryParameters userParams)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var pagedList = await _requestService.GetRequestsForManagementAsync(userId, userParams);

            // Create a metadata object
            var metadata = new
            {
                pagedList.TotalCount,
                pagedList.PageSize,
                pagedList.CurrentPage,
                pagedList.TotalPages,
                pagedList.HasNext,
                pagedList.HasPrevious
            };

            // Add to Headers (Angular can read this easily)
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(metadata));

            return Ok(pagedList); // Returns just the array of data
        }
        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpGet("recent")]
        public async Task<IActionResult> GetNotAssignedRequests([FromQuery] int numberOfRequests)
        {
            var recentRequests = await _unitOfWork.Request.dbSet
                .Where((r) => r.RegisteredRequest == null)
                .OrderByDescending(r => r.CreatedAt)
                .Take(numberOfRequests)
                .Select(r => new
                {
                    requestId = r.Id,
                    title = r.Title,
                    type = r.Type,
                    createdBy = r.CreatedBy.FullName,
                    createdAt = r.CreatedAt,

                })
                .ToListAsync();
            return Ok(recentRequests);
                                                          
        }

        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpGet("stats/chart/madevsdone/{period}")]
        [HttpGet("stats/created-vs-done/{period}")]
        public async Task<IActionResult> GetMadeRequestsCountVsDoneRequestsCountInAPeriod(
    [FromRoute] QueryPeriod period)
        {
            // 1. Get the offset from your service (It already handles the -1 inversion)
            int offsetToAdd = _userContext.TimezoneOffsetInMinutes;

            // 2. Determine "Now" in the USER'S local time
            var userNow = _userContext.ConvertToUserLocalTime(DateTime.UtcNow);

            // 3. Get the buckets (Labels/Ranges) based on Local Time
            var buckets = GetChartRanges(period, userNow);

            // 4. Calculate the total Span (Min Local Date to Max Local Date)
            var minLocal = buckets.First().From;
            var maxLocal = buckets.Last().To;

            // 5. Convert bounds back to UTC for the Database Query
            // Logic: Local - Offset = UTC
            var minUtc = minLocal.AddMinutes(-offsetToAdd);
            var maxUtc = maxLocal.AddMinutes(-offsetToAdd);

            // 6. SINGLE Database Query: Get all raw data in this range
            var rawDataUtc = await _unitOfWork.Request.dbSet
                .Where(x => x.CreatedAt >= minUtc && x.CreatedAt <= maxUtc)
                .Select(x => new { x.CreatedAt, x.Status })
                .ToListAsync();

            // 7. In-Memory Grouping
            var chartData = buckets.Select(bucket =>
            {
                return new
                {
                    Label = bucket.Label,

                    // Count requests created in this bucket that are NOT done
                    MadeValue = rawDataUtc.Count(req =>
                    {
                        var localDate = _userContext.ConvertToUserLocalTime(req.CreatedAt);
                        return localDate >= bucket.From &&
                               localDate <= bucket.To &&
                               req.Status != RequestStatus.Done;
                    }),

                    // Count requests created in this bucket that ARE done
                    DoneValue = rawDataUtc.Count(req =>
                    {
                        var localDate = _userContext.ConvertToUserLocalTime(req.CreatedAt);
                        return localDate >= bucket.From &&
                               localDate <= bucket.To &&
                               req.Status == RequestStatus.Done;
                    })
                };
            });

            return Ok(chartData);
        }

        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpGet("stats/new/{period}")]
        public async Task<IActionResult> GetNewRequestsCountInAPeriod([FromRoute] QueryPeriod period)
        {
            // 1. Get Current Bounds
            var currentBounds = GetQueryPeriodBounds(period);

            // 2. Get Previous Bounds (to calculate change)
            var previousBounds = GetPreviousBounds(period, currentBounds.From);

            // 3. Run Queries (It is best to run them Asynchronously)
            // Note: EF Core translates simple date comparisons efficiently.
            var currentCount = await _unitOfWork.Request
                .CountAsync((r) => r.CreatedAt >= currentBounds.From && r.CreatedAt <= currentBounds.To && r.Status == RequestStatus.UnderReview);

            var previousCount = await _unitOfWork.Request
                .CountAsync((r) => r.CreatedAt >= previousBounds.From && r.CreatedAt <= previousBounds.To && r.Status == RequestStatus.UnderReview);

            // 3.5 Total count of current under review requests
            var totalCount = await _unitOfWork.Request.CountAsync(r => r.Status == RequestStatus.UnderReview);

            // 4. Calculate Percentage
            double percentage = 0;

            if (previousCount == 0)
            {
                // If previous was 0 and current is > 0, technically it's 100% increase (or infinite).
                // Usually handled as 100 if current > 0, else 0.
                percentage = currentCount > 0 ? 100 : 0;
            }
            else
            {
                percentage = ((double)(currentCount - previousCount) / previousCount) * 100;
            }

            // 5. Prepare DTO
            var dto = new StatDto
            {
                Count = totalCount,
                ChangePercentage = Math.Round(Math.Abs(percentage), 2),
                Increased = percentage >= 0
            };

            return Ok(dto);
        }

        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpGet("stats/done/{period}")]
        public async Task<IActionResult> GetDoneRequestsInAPeriod([FromRoute] QueryPeriod period)
        {
            var currentBounds = GetQueryPeriodBounds(period);
            var previousBounds = GetPreviousBounds(period, currentBounds.From);

            var currentCount = await _unitOfWork.Request
            .CountAsync((r) => r.Logs.Any((l) => 
            l.NewStatus == RequestStatus.Done 
            && l.CreatedAt >= currentBounds.From 
            && l.CreatedAt <= currentBounds.To));

            var previousCount = await _unitOfWork.Request
            .CountAsync((r) => r.Logs.Any((l) =>
            l.NewStatus == RequestStatus.Done
            && l.CreatedAt >= previousBounds.From
            && l.CreatedAt <= previousBounds.To));

            double percentage = 0;

            if (previousCount == 0)
            {
                // If previous was 0 and current is > 0, technically it's 100% increase (or infinite).
                // Usually handled as 100 if current > 0, else 0.
                percentage = currentCount > 0 ? 100 : 0;
            }
            else
            {
                percentage = ((double)(currentCount - previousCount) / previousCount) * 100;
            }

            // 5. Prepare DTO
            var dto = new StatDto
            {
                Count = currentCount,
                ChangePercentage = Math.Round(Math.Abs(percentage), 2),
                Increased = percentage >= 0
            };

            return Ok(dto);
        }

        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpGet("stats/active/{period}")]
        public async Task<IActionResult> GetActiveRequestsInAPeriod([FromRoute] QueryPeriod period)
        {
            // 1. Get Current Bounds
            var currentBounds = GetQueryPeriodBounds(period);

            // 2. Get Previous Bounds (to calculate change)
            var previousBounds = GetPreviousBounds(period, currentBounds.From);

            // 3. Run Queries (It is best to run them Asynchronously)
            // Note: EF Core translates simple date comparisons efficiently.
            var currentCount = await _unitOfWork.Request
                .CountAsync((r) => r.Votes.Any((v) => v.VotedAt >= currentBounds.From && v.VotedAt <= currentBounds.To));

            var previousCount = await _unitOfWork.Request
                .CountAsync((r) => r.Votes.Any((v) => v.VotedAt >= previousBounds.From && v.VotedAt <= previousBounds.To));

            // 4. Calculate Percentage
            double percentage = 0;

            if (previousCount == 0)
            {
                // If previous was 0 and current is > 0, technically it's 100% increase (or infinite).
                // Usually handled as 100 if current > 0, else 0.
                percentage = currentCount > 0 ? 100 : 0;
            }
            else
            {
                percentage = ((double)(currentCount - previousCount) / previousCount) * 100;
            }

            // 5. Prepare DTO
            var dto = new StatDto
            {
                Count = currentCount,
                ChangePercentage = Math.Round(Math.Abs(percentage), 2),
                Increased = percentage >= 0
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
            var user = await _userManager.FindByIdAsync(userId);
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
            if (dto.ContributerPhoneNumber != null || dto.ContributerPhoneNumber != "")
            {
                user.PhoneNumber = dto.ContributerPhoneNumber;
                var result = await _userManager.UpdateAsync(user);
            }

            await _unitOfWork.Save();


            // Handle file upload
            if (dto.Attachments != null && dto.Attachments.Count > 0)
            {
                await SaveRequestAttachments(request.Id, dto.Attachments);    
            }
            await _unitOfWork.Save();
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

        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpPost("review/{id}")]
        public async Task<IActionResult> ReviewRequest([FromRoute] int id, [FromBody] ReviewRequestDto dto)
        {
            // 1. Get the Current Admin ID from the Token
            var currentAdminId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // 2. Fetch the Request
            var request = await _unitOfWork.Request.Get(r => r.Id == id, null, true);
            if (request == null) return NotFound();

            // 3. EXECUTE LOGIC (All in memory first)

            // A. Assign the Admin

            var assignment = new RegisteredRequest
            {
                RequestId = id,
                AssignedToId = currentAdminId,
                RegisteredAt = DateTime.UtcNow,
            };

            await _unitOfWork.RegisteredRequest.Add(assignment);

            // B. Change Status
            request.Status = dto.NewStatus;

            // C. Create the Log
            // If comment is null (Quick Action), generate a system message.
            string logComment = !string.IsNullOrEmpty(dto.Comment)
                ? dto.Comment
                : $"Quick {dto.NewStatus} action via Dashboard";

            var log = new RequestLog
            {
                RequestId = id,
                NewStatus = dto.NewStatus,
                Comment = logComment,
                IsPublic = true,
                CreatedAt = DateTime.UtcNow,
                CreatedById = currentAdminId,
            };

            await _unitOfWork.RequestLog.Add(log);

            // 4. Save Changes (Transaction commits here)
            await _unitOfWork.Save();

            return Ok(new { message = "Request reviewed successfully" });
        }
        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpPost("assign/{requestId}")]
        public async Task<IActionResult> AssignAdmin([FromRoute] int requestId)
        {
            var currentAdminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (currentAdminId == null) { return Unauthorized(); }

            var request = await _unitOfWork.Request.Get(r => r.Id == requestId, null, true);
            if (request == null) return NotFound("Request ID not found.");

            var assignment = new RegisteredRequest
            {
                RequestId = requestId,
                AssignedToId = currentAdminId,
                RegisteredAt = DateTime.UtcNow,
            };

            try
            {
                // 2. Attempt to Insert
                await _unitOfWork.RegisteredRequest.Add(assignment);
                request.IsRegistered = true;
                await _unitOfWork.Save();

                // If we reach this line, YOU are the winner.
                return Ok(new { message = "Request successfully assigned to you." });
            }
            catch (DbUpdateException) // This catches SQL errors
            {
                return Conflict(new { message = "This request is already assigned to another admin." });
            }

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue("role");
            var request = await _unitOfWork.Request.Get(r => r.Id == id, "Files", true);
            if (request == null)
                return NotFound();
            if (request.CreatedById != userId && userRole != "Admin" && userRole != "SuperAdmin")
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
            //To do: Delete associated DetailedRequest if exists, RequestLogs, RegisteredRequest etc.
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
