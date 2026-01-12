using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RightBrothersProduction.API.DTOs;
using RightBrothersProduction.DataAccess.Repositories.IRepositories;
using RightBrothersProduction.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using static RightBrothersProduction.Models.RequestModels;

namespace RightBrothersProduction.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VoteController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public VoteController(IUnitOfWork unitOfWork, IWebHostEnvironment env)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpGet]
        public async Task<IActionResult> GetVotes()
        {
            var votes = await _unitOfWork.Vote.GetAll();
            return Ok(votes);
        }

        [HttpGet("{id}")]
        public IActionResult GetVote(int id)
        {
            var vote = _unitOfWork.Vote.Get(v => v.Id == id);
            if (vote == null)
            {
                return NotFound();
            }
            return Ok(vote);
        }

        [HttpGet("request/{id}")]
        public async Task<IActionResult> GetVotersByRequestId([FromRoute]int id)
        {
            var votersQuery = _unitOfWork.Vote.dbSet.Where(v => v.RequestId == id).Select(v => new VoterDto
            {
                Id = v.User.Id,
                Name = v.User.FullName,
                PictureUrl = v.User.ProfilePictureUrl,
            });
            var voters = await votersQuery.ToListAsync();
            return Ok(voters);
        }

        [HttpGet("checkvoted/{id}")]
        public async Task<IActionResult> CheckIfVoted(int id) {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _unitOfWork.Vote.dbSet.Where(v => v.RequestId == id).AnyAsync(v => v.UserId == userId);
            return Ok(result);
        }

        

        [HttpPost]
        public async Task<IActionResult> CreateVote([FromBody] CreateVoteDto voteDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var existingVote = await _unitOfWork.Vote.Get(vote => vote.RequestId == voteDto.RequestId && vote.UserId == userId);

            var request = await _unitOfWork.Request.Get(r => r.Id == voteDto.RequestId, tracked: true);
            if (request == null) return NotFound("Request Not Found");
            if (request.Status == RequestStatus.UnderReview)
            {
                return BadRequest("Cannot vote on requests that are under review.");
            }

            if (existingVote != null)
            {
                // --- CASE: UN-VOTE ---
                _unitOfWork.Vote.Remove(existingVote);
                request.VotesCount--; // Decrement counter
            }
            else
            {
                // --- CASE: VOTE ---
                var newVote = new Vote
                {
                    RequestId = voteDto.RequestId,
                    UserId = userId,
                    VotedAt = DateTime.UtcNow
                };
                await _unitOfWork.Vote.Add(newVote);
                request.VotesCount++; // Increment counter
            }

            await _unitOfWork.Save();

            // Return the NEW count so the frontend can update immediately
            return Ok(request.VotesCount);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVote(int id)
        {
            var vote = await _unitOfWork.Vote.Get(v => v.Id == id, "Request", true);
            if (vote == null)
            {
                return NotFound();
            }
            _unitOfWork.Vote.Remove(vote);
            vote.Request.VotesCount--; // Decrement counter
            await _unitOfWork.Save();
            return Ok("Vote deleted successfully");
        }
    }
}
