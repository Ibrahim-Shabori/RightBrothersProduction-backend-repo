using RightBrothersProduction.API.DTOs;
using RightBrothersProduction.API.Helpers;

namespace RightBrothersProduction.API.Services.IServices
{
    public interface IUserService
    {
        // Define user-related service methods here
        Task<PagedList<UserManagementPageItemDto>> GetUsersForManagementAsync(UserManagementQueryParameters userParams);
        Task<int> GetRegularUsersCountAsync();
        Task<int> GetAdminUsersCountAsync();
        Task DemoteAdmin(string adminId);
        Task PromoteUserToAdmin(string userId);
        Task ToggleUserBanStatus(string userId);
        Task<bool> CanDemoteAsync(string loggedInUserId, string userToDemoteId);
        Task SetBanStatusAsync(string userId, bool isBanned);
    }
}
