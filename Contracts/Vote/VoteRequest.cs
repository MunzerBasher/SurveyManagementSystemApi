namespace SurveyManagementSystemApi.Contracts.Vote
{
    public class VoteRequest
    {
        public IEnumerable<VoteQuestionRequest> VoteAnswers { get; set; } = [];
    }

   

    public class VoteQuestionRequest
    {
        public int QuestionId { get; set; }
        public IEnumerable<int> QuestionAnswers { get; set; } = [];

    }




}
