using RightBrothersProduction.DataAccess.Data;
using RightBrothersProduction.DataAccess.Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RightBrothersProduction.DataAccess.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public ICategoryRepository Category { get; private set; }
        public IRequestRepository Request { get; private set; }
        public IDetailedRequestRepository DetailedRequest { get; private set; }
        public IRegisteredRequestRepository RegisteredRequest { get; private set; }
        public IRequestLogRepository RequestLog { get; private set; }
        public IVoteRepository Vote { get; private set; }
        public IApplicationUserRepository ApplicationUser { get; private set; }
        public IRequestFileRepository RequestFile { get; private set; }
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Category = new CategoryRepository(_db);
            Request = new RequestRepository(_db);
            DetailedRequest = new DetailedRequestRepository(_db);
            RegisteredRequest = new RegisteredRequestRepository(_db);
            RequestLog = new RequestLogRepository(_db);
            Vote = new VoteRepository(_db);
            ApplicationUser = new ApplicationUserRepository(_db);
            RequestFile = new RequestFileRepository(_db);
        }

        public Task<int> CompleteAsync()
        {
            return Task.FromResult(0);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
