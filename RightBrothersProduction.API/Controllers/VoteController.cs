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

        [HttpGet("request/{requestId}")]
        public IActionResult GetVotesByRequest(int requestId)
        {
            var votes = _unitOfWork.Vote.GetAll(v => v.RequestId == requestId);
            return Ok(votes);
        }

        [HttpPost]
        public async Task<IActionResult> CreateVote([FromBody] CreateVoteDto voteDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var existingVote = await _unitOfWork.Vote.Get(vote => vote.RequestId == voteDto.RequestId && vote.UserId == userId);

            var request = await _unitOfWork.Request.Get(r => r.Id == voteDto.RequestId, tracked: true);
            if (request == null) throw new Exception("Request not found");

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
            var vote = await _unitOfWork.Vote.Get(v => v.Id == id);
            if (vote == null)
            {
                return NotFound();
            }
            _unitOfWork.Vote.Remove(vote);
            await _unitOfWork.Save();
            return Ok("Vote deleted successfully");
        }
    }
}
