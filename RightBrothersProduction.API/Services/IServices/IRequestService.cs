using RightBrothersProduction.API.DTOs;
using RightBrothersProduction.API.Helpers;

namespace RightBrothersProduction.API.Services.IServices
{
    public interface IRequestService
    {
        Task<PagedList<RequestPageItemDto>> GetRequestsAsync(RequestQueryParameters userParams);
        Task<PagedList<RequestManagementPageItemDto>> GetRequestsForManagementAsync(string userId, RequestManagementQueryParameters userParams);
    }
}
