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
    public class RequestFileRepository : Repository<RequestFile>, IRequestFileRepository
    {
        private readonly ApplicationDbContext _db;
        public RequestFileRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
