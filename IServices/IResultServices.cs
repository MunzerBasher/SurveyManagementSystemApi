using SurveyManagementSystemApi.Contracts.Vote;

namespace SurveyManagementSystemApi.IServices
{
    public interface IResultServices
    {

        public Task<Result<IEnumerable<PollVoteResponse>>> PollResult(int pollId, CancellationToken cancellationToken = default);


        public Task<Result<IEnumerable<VotePerDayResponse>>> GetVotePerDay(int pollId, CancellationToken cancellationToken = default);


        public Task<Result<IEnumerable<VotePerQuestionResponse>>> GetVotePerQuestion(int pollId, CancellationToken cancellationToken = default);

    }
}
