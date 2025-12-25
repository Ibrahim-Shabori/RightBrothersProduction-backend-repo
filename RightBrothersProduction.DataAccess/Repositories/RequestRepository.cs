using Microsoft.EntityFrameworkCore;
using RightBrothersProduction.DataAccess.Data;
using RightBrothersProduction.DataAccess.Repositories.IRepositories;
using RightBrothersProduction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RightBrothersProduction.DataAccess.Repositories
{
    public class RequestRepository : Repository<Request>, IRequestRepository
    {
        private readonly ApplicationDbContext _db;
        public RequestRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public IList<Request> GetTopRequestsByVotes(int topN)
        {
            var requestsWithVoteCounts = _db.Requests
                .Select(r => new
                {
                    Request = r,
                    VoteCount = r.Votes.Count()
                })
                .OrderByDescending(rv => rv.VoteCount)
                .Take(topN)
                .Select(rv => rv.Request)
                .ToList();
            return requestsWithVoteCounts;
        }

        public Request? GetRequestWithVoters(int requestId) {
            return _db.Requests.Where(r => r.Id == requestId)
                               .Include(r => r.CreatedBy)
                               .Include(r => r.Category)
                               .Include(r => r.DetailedRequest)
                               .Include(r => r.Files)
                               .Include(r => r.Votes)
                               .ThenInclude(v => v.User)
                               .FirstOrDefault();
        }

        public async Task<bool> HasUserVoted(int requestId, string userId)
        {
            return await _db.Votes.AnyAsync(v => v.RequestId == requestId && v.UserId == userId);
        }
    }
}
