using static RightBrothersProduction.Models.RequestModels;

namespace RightBrothersProduction.API.DTOs
{
    public class RequestDetailsDto
    {
        public int Id {  get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public RequestStatus Status { get; set; }
        public bool IsDetailed { get; set; }
        public RequestType Type { get; set; }
        public  int VotesCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CategoryName { get; set; }
        public string CategoryColor { get; set; }
        public string CreatedByName { get; set; }
        public string CreatedById { get; set; }
        public string? CreatedByPictureUrl { get; set; }
        public IList<FileDto>? Files { get; set; }
        public int? UsageDuration { get; set; }
        public string? DetailedDescription { get; set; }
        public string? AdditionalNotes { get; set; }
        public string? UrgencyCause { get; set; }
        public string? ContributorPhoneNumber { get; set; }
        public string? ContributorEmail { get; set; }
    }
}
