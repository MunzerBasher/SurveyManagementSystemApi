using SurveyManagementSystemApi.Contracts.Vote;
 
namespace SurveyManagementSystemApi.Services
{
    public class ResultServices(AppDbContext appDbContext) : IResultServices
    {
        private readonly AppDbContext _appDbContext = appDbContext;


        public async Task<Result<IEnumerable<PollVoteResponse>>> PollResult(int pollId , CancellationToken cancellationToken = default)
        {
            var IsPollExist = await _appDbContext.polls.AnyAsync(p => p.Id == pollId && p.EndAt <= DateOnly.FromDateTime(DateTime.UtcNow));
            if (!IsPollExist)
                return Result<IEnumerable<PollVoteResponse>>.Failure<IEnumerable<PollVoteResponse>>(PollErrors.PollNotFound);
            var result = await _appDbContext.polls.Where(p => p.Id == pollId).Select(p => new PollVoteResponse
            {
                title = p.Title,
                voteResponses = p.Votes.
                Select(v => new VoteResponse
                {
                    VoterDate = v.SubmittedOn,
                    VoterName = $"{v.User.FirstName + " " + v.User.LastName}",
                    SelectedAnswer = v.Answers.Select(a =>
                    new QuestionAnswerResponse
                    {
                        Question = a.Question.Content,
                        QuestionAnswer = a.Answer.Content
                    }
                    ).ToList(),
                }
               )
            }
            ).AsNoTracking().ToListAsync();
            return  result is null?   Result<IEnumerable<PollVoteResponse>>.Failure<IEnumerable<PollVoteResponse>>(PollErrors.PollNotFound) :
                Result<IEnumerable<PollVoteResponse>>.Success<IEnumerable<PollVoteResponse>>(result);
        }


        public async Task<Result<IEnumerable<VotePerDayResponse>>> GetVotePerDay(int pollId, CancellationToken cancellationToken = default)
        {

            var IsPollExist = await _appDbContext.polls.AnyAsync(p => p.Id == pollId);
            if (!IsPollExist)
                return Result<IEnumerable<VotePerDayResponse>>.Failure<IEnumerable<VotePerDayResponse>>(PollErrors.PollNotFound);

             var result = await _appDbContext.Votes.Where(p => p.Id == pollId)
                .GroupBy(x => new {data = DateOnly.FromDateTime(x.SubmittedOn)})
                .Select( g => new VotePerDayResponse
                { 
                    title = g.Key.data, count = g.Count()
                }
                ).AsNoTracking().ToListAsync();
            return result is null ? Result<IEnumerable<VotePerDayResponse>>.Failure<IEnumerable<VotePerDayResponse>>(PollErrors.PollNotFound) :
                Result<IEnumerable<VotePerDayResponse>>.Success<IEnumerable<VotePerDayResponse>>(result);
        }


        public async Task<Result<IEnumerable<VotePerQuestionResponse>>> GetVotePerQuestion(int pollId, CancellationToken cancellationToken = default)
        {

            var IsPollExist = await _appDbContext.polls.AnyAsync(p => p.Id == pollId);
            if (!IsPollExist)
                return Result<IEnumerable<VotePerQuestionResponse>>.Failure<IEnumerable<VotePerQuestionResponse>>(PollErrors.PollNotFound);

            var result = await _appDbContext.VoteAnswers.Where(va => va.Vote.PollId == pollId).
                Select(q => q.Question).
                Select( q => new VotePerQuestionResponse
                {
                    Question = q.Content,
                    answerPerCounts = (
                                           from an in _appDbContext.Answers
                                           join va in _appDbContext.VoteAnswers
                                           on an.Id equals va.AnswerId
                                           where va.QuestionId == q.Id
                                           group an by an.Content into g
                                           select new AnswerPerCount
                                           {
                                               Answer = g.Key,
                                               Count = g.Count()
                                           }
                                       ).ToList()

                }).
                AsNoTracking().ToListAsync();
            return result is null ? Result<IEnumerable<VotePerQuestionResponse>>.Failure<IEnumerable<VotePerQuestionResponse>>(new Error("Not Found", StatusCodes.Status404NotFound)) :
                Result<IEnumerable<VotePerQuestionResponse>>.Success<IEnumerable<VotePerQuestionResponse>>(result);
        }

    }

}