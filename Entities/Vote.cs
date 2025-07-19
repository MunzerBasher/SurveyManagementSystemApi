namespace SurveyManagementSystemApi.Entities
{
    public class Vote
    {

        public int Id { get; set; }
        public int PollId { get; set; }
        public string UserId { get; set; }
        public DateTime SubmittedOn { get; set; } = DateTime.UtcNow;
        public Poll Poll { get; set; } = default!;
        public UserIdentity User { get; set; } = default!;
        public ICollection<VoteAnswers> Answers { get; set; } = [];

    }
}
