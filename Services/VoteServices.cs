using SurveyManagementSystemApi.Contracts.Vote;


namespace SurveyManagementSystemApi.Services
{
    public class VoteServices(AppDbContext AppContext) : IVoteServices
    {
        private readonly AppDbContext _AppDbContext = AppContext;

        public async Task<Result> AddAsync(int pollId, string userId, VoteRequest voteRequest, CancellationToken cancellationToken = default)
        {
            var Question = await _AppDbContext.Votes.FirstOrDefaultAsync(x => x.UserId == userId && x.PollId == pollId);
            if (Question is not null)
                return Result.Failure(VoteErrors.DuplicatedVote);
            var polls = await _AppDbContext.polls
              .AnyAsync(x => x.Id == pollId && x.IsPublished && x.EndAt > DateOnly.FromDateTime(DateTime.UtcNow) && x.StartAt <= DateOnly.FromDateTime(DateTime.UtcNow));
            if (!polls)
                return Result.Failure(VoteErrors.InvalidQuestions);
            
            foreach ( var v in voteRequest.VoteAnswers)
            {
                var answer = await (
                             from a in _AppDbContext.Answers 
                             join q in _AppDbContext.Questions
                             on a.QuestionId equals q.Id
                             where v.QuestionId == q.Id
                             select a.Id
                             ).ToListAsync();
                foreach(var a in v.QuestionAnswers)
                {
                    if(!answer.Contains(a))
                        return Result.Failure(VoteErrors.InvalidAnswerForQuestions);
                }
            }
            var vote = new Vote
            {
                PollId = pollId,
                SubmittedOn = DateTime.UtcNow,
                UserId = userId,
            };
            await _AppDbContext.Votes.AddAsync(vote);
            await _AppDbContext.SaveChangesAsync(cancellationToken);
            IList<VoteAnswers> Answers = [];
            foreach ( var v in voteRequest.VoteAnswers)
            {
                foreach(var a in v.QuestionAnswers )
                {
                    Answers.Add(new VoteAnswers {VoteId = vote.Id,AnswerId = a, QuestionId = v.QuestionId });
                }
            }
            await _AppDbContext.AddRangeAsync(Answers,cancellationToken); 
            await _AppDbContext.SaveChangesAsync(cancellationToken);
            return Result.Success();

        }
    }

}