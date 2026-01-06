using RightBrothersProduction.API.DTOs;
using RightBrothersProduction.API.Helpers;
using RightBrothersProduction.API.Services.IServices;
using RightBrothersProduction.DataAccess.Repositories.IRepositories;
using RightBrothersProduction.Models;
using RightBrothersProduction.Models.Interfaces;
using static RightBrothersProduction.Models.RequestModels;

namespace RightBrothersProduction.API.Services
{
    public class RequestService : IRequestService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContext _userContext;
        public RequestService(IUnitOfWork unitOfWork, IUserContext userContext)
        {
            _unitOfWork = unitOfWork;    
            _userContext = userContext;
        }
        public async Task<PagedList<RequestPageItemDto>> GetRequestsAsync(RequestQueryParameters userParams)
        {
            // Let's build the query step by step. first initializing the query to be on the whole to-review requests.
            var query = _unitOfWork.Request.dbSet.Where(r => r.Status == RequestStatus.UnderReview).AsQueryable();

            // 1) the query should determine the types needed as following:
            // .. if null: all, if -1: Feauters Categories, -2: Bugs Categories,
            // .. any other number, it is the id of some needed category.
            if (userParams.Types != null) query = query.Where(r => userParams.Types.Contains(r.Category.Id));

            // 2) the query should look for the SortField, and SortOrder, and based on them, sort the rows.
            // .. the only sortable columns are in my openion: createdAt and the types also. sorting based on
            // .. createdAt is pretty clear, while based on type, as for me, will show the detailed ones first. (Asc).
            switch (userParams.SortField)
            {
                case "createdAt":
                    query = (userParams.SortOrder == 1) ?
                        query.OrderBy(r => r.CreatedAt) 
                        : query.OrderByDescending(r => r.CreatedAt);
                    break;
                case "type":
                    query = (userParams.SortOrder == 1)
                        // Case 1: Show Detailed First (True first)
                        ? query.OrderByDescending(r => r.DetailedRequest != null)
                               .ThenByDescending(r => r.CreatedAt)

                        // Case 2: Show Regular First (False first)
                        : query.OrderBy(r => r.DetailedRequest != null)
                               .ThenByDescending(r => r.CreatedAt);
                    break;
                default:
                    query = query.OrderBy(r => (r.DetailedRequest != null)).ThenByDescending(r => r.CreatedAt);
                    break;
            }

            // 3) next, look for the searchQuery, and give only the outputs that has a title contains these words,
            // .. till using the elasticSearch later.
            if (userParams.Filter != null && userParams.Filter != "")
            {
                query = query.Where(r => r.Title.ToLower().Contains(userParams.Filter.ToLower()));
            }

            // 4) determining the dto format I need to send through the api call respond.
            var dtoFormat = query.Select(r => new RequestPageItemDto {
                Id = r.Id,
                Title = r.Title,
                Description = r.Description,
                Type = r.Type,
                CategoryName = r.Category.Name,
                CategoryId = r.CategoryId,
                IsDetailed = (r.DetailedRequest != null),
                CreatedAt = _userContext.ConvertToUserLocalTime(r.CreatedAt),
                CreatedBy = r.CreatedBy.FullName.Split()[0],
                CreatedByProfilePicture = r.CreatedBy.ProfilePictureUrl
            
            });
            return await PagedList<RequestPageItemDto>.CreateAsync(dtoFormat, userParams.PageNumber, userParams.PageSize);
        }
    
        public async Task<PagedList<RequestManagementPageItemDto>> GetRequestsForManagementAsync(string userId, RequestManagementQueryParameters userParams)
        {
            // Let's build the query step by step. first initializing the query to be on the whole requests.
            var query = _unitOfWork.Request.dbSet.AsQueryable();
            query = query.Where(r => r.RegisteredRequest != null && r.RegisteredRequest.AssignedToId == userId);
            // 1) we should add the trend value for each request. it is calculated based on
            // .. the number of votes in the query time period. lets get the bounds of this period:
            (DateTime From, DateTime To) = PeriodModels.GetQueryPeriodBounds(userParams.Period);
            // .. now, we can use these bounds to get the trends count for each request.
            // .. and project the query to include trends count, logs, last updated at.
            var queryDto = query.Select(r => new
            {
                Id = r.Id,
                Title = r.Title,
                Description = r.Description,
                Type = r.Type,
                Status = r.Status,
                IsDetailed = (r.DetailedRequest != null),
                CreatedBy = r.CreatedBy,
                LastUpdatedAt = r.Logs.OrderByDescending(l => l.CreatedAt).FirstOrDefault().CreatedAt,
                RequestLogs = r.Logs.Select(l => new RequestLogForAdminsDto { NewStatus = l.NewStatus, Comment = l.Comment,  CreatedAt = l.CreatedAt, IsPublic = l.IsPublic}),
                Votes = r.VotesCount,
                TrendsCount = r.Votes.Count(v => v.VotedAt >= From && v.VotedAt <= To)
            });


            // 2) the query should look for the SortField, and SortOrder, and based on them, sort the rows.
            // .. the only sortable columns are in my openion: LastUpdatedAt and votes and trendsCount. sorting based on
            // .. createdAt is pretty clear.
            switch (userParams.SortField)
            {
                case "updatedAt":
                    queryDto = (userParams.SortOrder == 1) ?
                        queryDto.OrderBy(r => r.LastUpdatedAt)
                        : queryDto.OrderByDescending(r => r.LastUpdatedAt);
                    break;
                case "votes":
                    queryDto = (userParams.SortOrder == 1)
                        ? queryDto.OrderBy(r => r.Votes)
                        : queryDto.OrderByDescending(r => r.Votes);
                    break;
                case "trend":
                    queryDto = (userParams.SortOrder == 1)
                        ? queryDto.OrderBy(r => r.TrendsCount)
                        : queryDto.OrderByDescending(r => r.TrendsCount);
                    break;
                default:
                    queryDto = (userParams.SortOrder == 1)
                        ? queryDto.OrderBy(r => r.TrendsCount)
                        : queryDto.OrderByDescending(r => r.TrendsCount);
                    break;
            }
            // 3) next, look for the searchQuery, and give only the outputs that has a title contains these words,
            // .. till using the elasticSearch later.
            if (userParams.Filter != null && userParams.Filter != "")
            {
                queryDto = queryDto.Where(r => r.Title.ToLower().Contains(userParams.Filter.ToLower()));
            }
            // 4) determining the dto format I need to send through the api call respond.
            var dtoFormat = queryDto.Select(r => new RequestManagementPageItemDto
            {
                Id = r.Id,
                Title = r.Title,
                Description = r.Description,
                Type = r.Type,
                Status = r.Status,
                IsDetailed = r.IsDetailed,
                VotesCount = r.Votes,
                TrendsCount = r.TrendsCount,
                LastUpdatedAt = r.LastUpdatedAt,
                CreatedBy = r.CreatedBy.FullName.Split()[0],
                Logs = r.RequestLogs.OrderByDescending(l => l.CreatedAt).ToList()
            });

            var paginatedList = await PagedList<RequestManagementPageItemDto>.CreateAsync(dtoFormat, userParams.PageNumber, userParams.PageSize);
            foreach (var item in paginatedList) { 
                item.LastUpdatedAt = _userContext.ConvertToUserLocalTime(item.LastUpdatedAt);
                foreach (var log in item.Logs)
                {
                    log.CreatedAt = _userContext.ConvertToUserLocalTime(log.CreatedAt);
                }
            }
            return paginatedList;
        }

    }
}
