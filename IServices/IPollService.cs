using SurveyManagementSystemApi.Contracts.Poll;

namespace SurveyManagementSystemApi.IServices
{
    public interface IPollService
    {

        Task<Result<IEnumerable<PollResponse>>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<Result<IEnumerable<PollResponse>>> GetCurrentAsync(CancellationToken cancellationToken = default);

        Task<Result<Poll>>? GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default);

        Task<Result<Poll>> UpdateAsync(int id, PollRequest poll, CancellationToken cancellationToken = default);

        Task<Result> AddAsync(PollRequest poll, CancellationToken cancellationToken = default);

        Task<Result> ToggleStatus(int pollId, CancellationToken cancellationToken = default);

    }
}