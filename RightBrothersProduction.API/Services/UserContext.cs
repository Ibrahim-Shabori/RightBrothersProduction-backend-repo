using RightBrothersProduction.Models.Interfaces;

namespace RightBrothersProduction.API.Services
{
    public class UserContext : IUserContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int TimezoneOffsetInMinutes
        {
            get
            {
                var context = _httpContextAccessor.HttpContext;
                if (context != null &&
                    context.Request.Headers.TryGetValue("X-Timezone-Offset", out var offsetString))
                {
                    if (int.TryParse(offsetString, out int offset))
                    {
                        // JS getTimezoneOffset is inverted (Positive = West, Negative = East)
                        // Usually we want to invert it back to add to UTC
                        return offset * -1;
                    }
                }
                return 0; // Default to UTC if missing
            }
        }

        public DateTime ConvertToUserLocalTime(DateTime? utcDate)
        {
            return utcDate?.AddMinutes(TimezoneOffsetInMinutes)?? DateTime.MinValue;
        }
    }
}
