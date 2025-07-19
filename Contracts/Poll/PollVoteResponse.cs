using SurveyManagementSystemApi.Contracts.Vote;

namespace SurveyManagementSystemApi.Contracts.Poll
{
    public class PollVoteResponse
    {

        public string title { get; set; } = string.Empty;
        public IEnumerable<VoteResponse> voteResponses { get; set; } = default!;

    }
}
