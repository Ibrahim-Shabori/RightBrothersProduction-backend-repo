using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RightBrothersProduction.DataAccess.Repositories.IRepositories
{
    public interface IUnitOfWork
    {
        IRequestRepository Request { get; }
        IRegisteredRequestRepository RegisteredRequest { get; }
        IDetailedRequestRepository DetailedRequest { get; }
        IRequestLogRepository RequestLog { get; }
        IVoteRepository Vote { get; }
        ICategoryRepository Category { get; }
        IApplicationUserRepository ApplicationUser { get; }
        IRequestFileRepository RequestFile { get; }
        Task Save();
    }
}
