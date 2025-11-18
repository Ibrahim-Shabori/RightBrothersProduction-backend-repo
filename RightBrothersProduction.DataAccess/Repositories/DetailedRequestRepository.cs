using RightBrothersProduction.DataAccess.Data;
using RightBrothersProduction.DataAccess.Repositories.IRepositories;
using RightBrothersProduction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RightBrothersProduction.DataAccess.Repositories
{
    public class DetailedRequestRepository : Repository<DetailedRequest>, IDetailedRequestRepository
    {
        private readonly ApplicationDbContext _db;
        public DetailedRequestRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
