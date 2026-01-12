using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RightBrothersProduction.API.DTOs;
using RightBrothersProduction.DataAccess.Repositories.IRepositories;
using RightBrothersProduction.Models;
using System.Security.Claims;

namespace RightBrothersProduction.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestLogController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public RequestLogController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager) 
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpGet("{requestId}")]
        public async Task<IActionResult> GetRequestLogs([FromRoute] int requestId) { 
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var request = await _unitOfWork.RegisteredRequest.Get(r => r.RequestId == requestId);
            if (request == null) { 
                return NotFound("Request not found or not assigned yet.");
            }

            if (userId != request.AssignedToId)
            {
                return Unauthorized("You are not assigned to this request.");
            }
            var requestLogs = await _unitOfWork.RequestLog.GetAllAsQueryable(l => l.RequestId == requestId);
            var logsDto = await requestLogs.Select(l => new
            {
                NewStatus = l.NewStatus,
                CreatedAt = l.CreatedAt,
                Comment = l.Comment,
            }).ToListAsync();
            return Ok(logsDto);
        }


        [HttpGet("request/{id}")]
        public async Task<IActionResult> GetRequestLogsForRequestDetailsPage([FromRoute]int id)
        {
            bool isAdmin = User.IsInRole("Admin") || User.IsInRole("SuperAdmin");
            
            var logsQuery = _unitOfWork.RequestLog.dbSet.Where(l => l.RequestId == id);
            if (isAdmin == false)
            {
                logsQuery = logsQuery.Where(l => l.IsPublic == true);
            }
            var nlogsQuery = logsQuery.Select(l => new RequestLogDetailsDto
            {
                Comment = l.Comment,
                CreatedAt = l.CreatedAt,
                IsPublic = l.IsPublic,
                LoggerName = l.CreatedBy.FullName,
                LoggerPictureUrl = l.CreatedBy.ProfilePictureUrl,
                NewStatus = l.NewStatus,
            });

            var logs = await nlogsQuery.ToListAsync();
            return Ok(logs);
        }

        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> CreateRequestLog([FromBody] CreateRequestLogDto logDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var request = await _unitOfWork.RegisteredRequest.Get(r => r.RequestId == logDto.RequestId, "Request", true);
            if (request == null)
            {
                return NotFound("Request not found or not assigned yet.");
            }

            if (userId != request.AssignedToId)
            {
                return Unauthorized("You are not assigned to this request.");
            }
            
            var requestLog = new RequestLog { 
                RequestId = logDto.RequestId,
                NewStatus = logDto.NewStatus,
                Comment = logDto.Comment,
                IsPublic = logDto.IsPublic,
                CreatedAt = DateTime.UtcNow,
                CreatedById = userId
            };

            request.Request.Status = logDto.NewStatus;

            await _unitOfWork.RequestLog.Add(requestLog);
            await _unitOfWork.Save();

            return Ok();
        }
    }
}
