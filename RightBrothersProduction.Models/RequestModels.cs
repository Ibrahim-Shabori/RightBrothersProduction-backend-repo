

namespace RightBrothersProduction.Models
{
    public static class RequestModels
    {
        public enum RequestStatus
        {
            Rejected,
            UnderReview,
            Published,
            InConsideration,
            InProgress,
            Done
        }

        public enum RequestType
        {
            Regular,
            Detailed,
            Bug,
            DetailedBug
        }
    }
}
