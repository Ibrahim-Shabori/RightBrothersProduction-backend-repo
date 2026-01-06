using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RightBrothersProduction.API.DTOs;
using RightBrothersProduction.API.Services.IServices;
using RightBrothersProduction.Models;
using RightBrothersProduction.Models.Interfaces;
using System.Security.Claims;
using System.Text.Json;
using static RightBrothersProduction.Models.PeriodModels;

namespace RightBrothersProduction.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserService _userService;
        public UserController(UserManager<ApplicationUser> userManager, IUserService userService){
            _userManager = userManager;
            _userService = userService;
        }

        [HttpGet("stats/{period}")]
        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> GetUsersCountInAPeriod([FromRoute] QueryPeriod period){
            // 1. Get Current Bounds
            var currentBounds = GetQueryPeriodBounds(period);

            // 2. Get Previous Bounds (to calculate change)
            var previousBounds = GetPreviousBounds(period, currentBounds.From);

            // 3. Run Queries (It is best to run them Asynchronously)
            // Note: EF Core translates simple date comparisons efficiently.
            var currentCount = await _userManager.Users
                .CountAsync(u => u.DateJoined >= currentBounds.From && u.DateJoined <= currentBounds.To);

            var previousCount = await _userManager.Users
                .CountAsync(u => u.DateJoined >= previousBounds.From && u.DateJoined <= previousBounds.To);

            // 3.5. Count total users we have
            var totalCount = await _userManager.Users.CountAsync();

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

        [HttpGet("stats/effect/{period}")]
        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> GetUsersEffectInAPeriod([FromRoute] QueryPeriod period, [FromQuery] int numberOfUsers = 5)
        {
            var bounds = GetQueryPeriodBounds(period);

            var topUsers = await _userManager.Users
                .Select(user => new
                {
                    UserId = user.Id,
                    UserName = user.FullName,
                    ImageURL = user.ProfilePictureUrl,
                    RequestsGotVotes = user.RequestsCreated.Count(req => req.Votes.Any(v => v.VotedAt >= bounds.From &&
                                                                                            v.VotedAt <= bounds.To)),
                    ReceivedVotesCount = user.RequestsCreated
                        .Sum(req => req.Votes.Count(v => v.VotedAt >= bounds.From &&
                                                         v.VotedAt <= bounds.To)),
                    
                })
                .OrderByDescending(x => x.ReceivedVotesCount)
                .ThenBy(x => x.RequestsGotVotes)
                .Take(numberOfUsers)
                .ToListAsync();

            return Ok(topUsers);
        }

        [HttpGet("manage/pages")]
        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<ActionResult<IEnumerable<UserManagementPageItemDto>>> GetUsersToManage([FromQuery] UserManagementQueryParameters userParams)
        {
            var pagedList = await _userService.GetUsersForManagementAsync(userParams);

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

            // For pagination info in headers
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(metadata));

            return Ok(pagedList);
        }

        [HttpGet("stats/count")]
        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> GetUsersCountStats()
        {
            var adminCount = await _userService.GetAdminUsersCountAsync();
            var regularCount = await _userService.GetRegularUsersCountAsync();
            var dto = new
            {
                AdminUsersCount = adminCount,
                RegularUsersCount = regularCount
            };
            return Ok(dto);
        }


        // 1. Promote (POST) - Strict Security
        [HttpPost("{id}/promote")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Promote(string id)
        {
            await _userService.PromoteUserToAdmin(id);
            return NoContent();
        }

        // 2. Demote (POST) - Strict Security
        [HttpPost("{id}/demote")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Demote(string id)
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // Prevent demoting self or other SuperAdmins
            if (!await _userService.CanDemoteAsync(loggedInUserId, id))
                return Forbid();

            await _userService.DemoteAdmin(id);
            return NoContent();
        }

        // 3. Ban Status (PATCH) - Explicit State
        // Using PATCH implies "Partial Update"
        [HttpPatch("{id}/ban-status")]
        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> UpdateBanStatus(string id, [FromBody] BanStatusDto dto)
        {
            // Dto contains: public bool IsBanned { get; set; }
            await _userService.SetBanStatusAsync(id, dto.IsBanned);
            return NoContent();
        }



    }
}