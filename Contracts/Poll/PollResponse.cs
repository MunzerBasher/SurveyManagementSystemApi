namespace SurveyManagementSystemApi.Contracts.Poll
{
    public class PollResponse
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public bool IsPublished { get; set; }
        public DateOnly StartAt { get; set; }
        public DateOnly EndAt { get; set; }
    }
}
