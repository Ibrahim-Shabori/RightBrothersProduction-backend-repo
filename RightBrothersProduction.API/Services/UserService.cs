using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RightBrothersProduction.API.DTOs;
using RightBrothersProduction.API.Helpers;
using RightBrothersProduction.API.Services.IServices;
using RightBrothersProduction.DataAccess.Repositories.IRepositories;
using RightBrothersProduction.Models;
using RightBrothersProduction.Models.Interfaces;
using System.Threading.Tasks;

namespace RightBrothersProduction.API.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserContext _userContext;
        private readonly IUnitOfWork _unitOfWork;
        public UserService(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork , IUserContext userContext)
        {
            _userManager = userManager;
            _userContext = userContext;
            _unitOfWork = unitOfWork;

        }

        public async Task<int> GetAdminUsersCountAsync()
        {
            return await _unitOfWork.ApplicationUser.dbSet.CountAsync(u => u.UserRoles.Any(ur => (ur.Role.Name == "SuperAdmin") || (ur.Role.Name == "Admin")));
        }

        public async Task<int> GetRegularUsersCountAsync()
        {
            return await _unitOfWork.ApplicationUser.dbSet.CountAsync(u => u.UserRoles.Any(ur => (ur.Role.Name == "User")));
        }

        public async Task<PagedList<UserManagementPageItemDto>> GetUsersForManagementAsync(UserManagementQueryParameters userParams)
        {
            // 1) Specify the base query based on user type
            var query = (userParams.usersType == "Admins")?
                _unitOfWork.ApplicationUser.dbSet.Where(u => u.UserRoles.Any(ur => (ur.Role.Name == "SuperAdmin") || (ur.Role.Name == "Admin")))
                : _unitOfWork.ApplicationUser.dbSet.Where(u => u.UserRoles.Any(ur => (ur.Role.Name == "User")));

            // 2) Project to UserManagementPageItemDto based on user type (to determine performance score calculation)
            var projectedQuery = (userParams.usersType == "Admins") ?
                query.Select(u => new UserManagementPageItemDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    ProfilePictureUrl = u.ProfilePictureUrl,
                    JoinedAt = u.DateJoined,
                    IsActive = !u.IsBanned,
                    Role = u.UserRoles.FirstOrDefault().Role.Name,
                    PerformanceScore = u.RequestLogs.Count(),
                    LastActivity = u.RequestLogs.OrderByDescending(rl => rl.CreatedAt).FirstOrDefault().CreatedAt
                })
                :
                query.Select(u => new UserManagementPageItemDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    ProfilePictureUrl = u.ProfilePictureUrl,
                    JoinedAt = u.DateJoined,
                    IsActive = !u.IsBanned,
                    Role = u.UserRoles.FirstOrDefault().Role.Name,
                    PerformanceScore = u.RequestsCreated.Sum(r => r.VotesCount),
                    LastActivity = u.Votes.OrderByDescending(v => v.VotedAt).FirstOrDefault().VotedAt
                });

            // 2.5) if just active users are requested, filter them
            if (userParams.isActive == true)
            {
                projectedQuery = projectedQuery.Where(u => u.IsActive == true);
            } else if (userParams.isActive == false)
            {
                projectedQuery = projectedQuery.Where(u => u.IsActive == false);
            }

            // 3) Order based on sorting parameters
            projectedQuery = userParams.SortField switch
            {
                "performance" => (userParams.SortOrder == -1)? projectedQuery.OrderByDescending(u => u.PerformanceScore) : projectedQuery.OrderBy(u => u.PerformanceScore),
                "joinDate" => (userParams.SortOrder == -1)? projectedQuery.OrderByDescending(u => u.JoinedAt) : projectedQuery.OrderBy(u => u.JoinedAt),
                "lastActivity" => (userParams.SortOrder == -1)? projectedQuery.OrderByDescending(u => u.LastActivity): projectedQuery.OrderBy(u => u.LastActivity),
                //"isActive" => (userParams.SortOrder == -1)? projectedQuery.OrderByDescending(u => (u.IsActive == true)) : projectedQuery.OrderBy(u => u.IsActive == true),
                _ => projectedQuery.OrderByDescending(u => u.PerformanceScore),
            };

            // 4) Filter with the search term if provided
            if (!string.IsNullOrEmpty(userParams.Filter))
            {
                var lowerSearchTerm = userParams.Filter.ToLower();
                projectedQuery = projectedQuery.Where(u =>
                    u.FullName.ToLower().Contains(lowerSearchTerm) ||
                    u.Email.ToLower().Contains(lowerSearchTerm)
                );
            }

            var pagedList = await PagedList<UserManagementPageItemDto>.CreateAsync(projectedQuery, userParams.PageNumber, userParams.PageSize);
            foreach (var item in pagedList)
            {

                item.LastActivity = (item.LastActivity != null) ? _userContext.ConvertToUserLocalTime(item.LastActivity) : DateTime.MinValue;
                item.JoinedAt = _userContext.ConvertToUserLocalTime(item.JoinedAt);
            }

            // 5) Return the paged list
            return pagedList;
        }
    
        public async Task DemoteAdmin(string adminId) {
            var adminUser = await _userManager.FindByIdAsync(adminId);
            await _userManager.RemoveFromRoleAsync(adminUser, "Admin");
            await _userManager.AddToRoleAsync(adminUser, "User");
            await _userManager.UpdateAsync(adminUser);
            return;
        }

        public async Task PromoteUserToAdmin(string userId) {
            var user = await _userManager.FindByIdAsync(userId);
            await _userManager.RemoveFromRoleAsync(user, "User");
            await _userManager.AddToRoleAsync(user, "Admin");
            await _userManager.UpdateAsync(user);
            return;
        }

        public async Task ToggleUserBanStatus(string userId) {
            var user = await _userManager.FindByIdAsync(userId);
            user.IsBanned = !user.IsBanned;
            await _userManager.UpdateAsync(user);
            return;
        }

        public async Task<bool> CanDemoteAsync(string loggedInUserId, string userToDemoteId)
        {
            if (loggedInUserId == userToDemoteId)
                return false; // Cannot demote self
            var userToDemote = _unitOfWork.ApplicationUser.dbSet.Where(u => u.Id == userToDemoteId);
            if (userToDemote == null) return false;

            userToDemote = userToDemote.Include(u => u.UserRoles).ThenInclude(ur => ur.Role);
            var user = await userToDemote.FirstOrDefaultAsync();
            if (user.UserRoles.Any(us => us.Role.Name == "SuperAdmin"))
                return false; // SuperAdmin cannot be demoted
            return true;
        }

        public async Task SetBanStatusAsync(string userId, bool isBanned)
        {
            var user = (await _userManager.FindByIdAsync(userId));
            if (user == null) return;
            user.IsBanned = isBanned;
            await _userManager.UpdateAsync(user);
            return;
        }
    }
}
