namespace SurveyManagementSystemApi.Contracts.Vote
{
    public class VotePerDayResponse
    {
        public DateOnly title { get; set; }
        public int count { get; set; }
    }
}
