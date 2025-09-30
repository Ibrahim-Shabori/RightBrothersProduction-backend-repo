

namespace RightBrothersProduction.Models
{
    public static class RequestModels
    {
        public enum RequestType
        {
            Feature,
            Bug
        }

        public enum RequestStatus
        {
            UnderReview,
            Published,
            InConsideration,
            InProgress,
            Done
        }
    }
}
