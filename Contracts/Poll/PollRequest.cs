namespace SurveyManagementSystemApi.Contracts.Poll
{
    public class PollRequest
    {
        public required string Title { get; set; }
        public required string Description { get; set; }
        public DateOnly StartAt { get; set; }
        public DateOnly EndAt { get; set; }
    }
}
