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
    }
}
