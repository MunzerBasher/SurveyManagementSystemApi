

namespace SurveyManagementSystemApi.IServices
{
    public interface IQuestion
    {
        Task<Result<IEnumerable<QuestionResponse>>> GetAllAsync(int pollId, CancellationToken cancellationToken = default);

        Task<Result<QuestionResponse>> GetAsync(int id, int questionId, CancellationToken cancellationToken = default);

        Task<Result> UpdateAsync(QuestionRequest request, int pollId, int id, CancellationToken cancellationToken = default);

        Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default);

        Task<Result> AddAsync(QuestionRequest response, CancellationToken cancellationToken = default);

        Task<Result> ToggleStatusAsync(int pollId, int id, CancellationToken cancellationToken = default);
        
        Task<Result<IEnumerable<QuestionResponse>>> AvalibleQuestionsAsync(int pollId, string userId, CancellationToken cancellationToken = default);


    }
}
