using static RightBrothersProduction.Models.PeriodModels;

namespace RightBrothersProduction.API.DTOs
{
    public class QueryParameters: PaginationParameters
    {
        public string? Filter { get; set; }
        public string? SortField { get; set; }
        public int SortOrder { get; set; }
    }

    public class RequestQueryParameters: QueryParameters
    {    
        public List<int>? Types { get; set; }
    }

    public class RequestManagementQueryParameters: QueryParameters
    {    
        public QueryPeriod Period { get; set; }
    }

    public class UserManagementQueryParameters : QueryParameters
    {
        public string? usersType { get; set; }
        public bool? isActive { get; set; }
    }
}
