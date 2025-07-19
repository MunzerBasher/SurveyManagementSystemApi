namespace SurveyManagementSystemApi.Contracts.Vote
{
    public class VoteResponse
    {
        public string VoterName { get; set; }
        public DateTime? VoterDate { get; set; }
        public IEnumerable<QuestionAnswerResponse> SelectedAnswer { get; set; }
    }
}
