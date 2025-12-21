using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RightBrothersProduction.API.DTOs;
using RightBrothersProduction.DataAccess.Repositories.IRepositories;
using RightBrothersProduction.Models;

namespace RightBrothersProduction.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoteController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public VoteController(IUnitOfWork unitOfWork, IWebHostEnvironment env)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpGet]
        public IActionResult GetVotes()
        {
            var votes = _unitOfWork.Vote.GetAll();
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
        public IActionResult CreateVote([FromBody] CreateVoteDto vote)
        {
            if (vote == null)
            {
                return BadRequest();
            }
            Vote newVote = new Vote
            {
                UserId = vote.UserId,
                RequestId = vote.RequestId,
                VotedAt = DateTime.UtcNow
            };
            _unitOfWork.Vote.Add(newVote);
            _unitOfWork.Save();
            return Ok(newVote);
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
