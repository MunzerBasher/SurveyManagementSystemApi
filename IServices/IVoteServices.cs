using SurveyManagementSystemApi.Contracts.Vote;

namespace SurveyManagementSystemApi.IServices
{
    public interface IVoteServices
    {

        public Task<Result> AddAsync(int pollId, string userId, VoteRequest voteRequest, CancellationToken cancellationToken = default );

    }
}
