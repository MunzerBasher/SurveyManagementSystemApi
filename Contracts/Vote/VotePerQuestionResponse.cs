namespace SurveyManagementSystemApi.Contracts.Vote
{
    public class VotePerQuestionResponse
    {
        public string Question { get; set; }

        public IEnumerable<AnswerPerCount> answerPerCounts { get; set; }
    }
}
