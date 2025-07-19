namespace SurveyManagementSystemApi.Services
{



    public class QuestionServices(AppDbContext appDbContext, ICacheService distributedCache,ILogger<QuestionServices> logger) : IQuestion
    {
        private readonly AppDbContext _AppDbContext = appDbContext;
        private readonly ILogger<QuestionServices> _logger = logger;
        private readonly ICacheService _distributedCache = distributedCache;
        private const string _QuestionscachePrefix = "Questionshw53#22";

        public async Task<Result> AddAsync(QuestionRequest Request, CancellationToken cancellationToken = default)
        {
            var IsPollExist = await _AppDbContext.polls.AnyAsync(x => x.Id == Request.pollId);
            if (!IsPollExist)
                return Result.Failure(PollErrors.PollNotFound);
            var IsContectExist = await _AppDbContext.Questions.AnyAsync(x => x.Content == Request.Contant && x.PollId == Request.pollId);
            if (IsContectExist)
                return Result.Failure(QuestionErrors.QuestionNotFound); 
            Question question = new Question
            {
                Content = Request.Contant,
                PollId = Request.pollId,
                Answers = Request.Answers.Select(x => new Answer { Content = x }).ToList()
            }; 
            var Question = await _AppDbContext.Questions.AddAsync(question);
            var IsAdded = await _AppDbContext.SaveChangesAsync();
            if(IsAdded > 0)
            {
                var cachekey = $"{_QuestionscachePrefix}_{Request.pollId}"; 
                await _distributedCache.RemoveAsync(cachekey);
            }

            return IsAdded > 0 ? Result.Success() : Result.Failure(QuestionErrors.InternalServerError);
        }


        public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var question = await _AppDbContext.Questions.FindAsync(id);
            if (question is null)
                return Result.Failure(QuestionErrors.QuestionNotFound);
           _AppDbContext.Questions.Remove(question);
           var IsDeleted = await _AppDbContext.SaveChangesAsync();
            if(IsDeleted > 0)
            {
                var cachekey = $"{_QuestionscachePrefix}_{question.PollId}";
                await _distributedCache.RemoveAsync(cachekey);

            }
            return IsDeleted > 0 ? Result.Failure(QuestionErrors.InternalServerError) : Result.Success();

        }

        public async Task<Result<IEnumerable<QuestionResponse>>> GetAllAsync(int pollId, CancellationToken cancellationToken = default)
        {
            var IsPollExist = await _AppDbContext.polls.AnyAsync(x => x.Id == pollId);
            if (!IsPollExist)
                return Result<IEnumerable<QuestionResponse>>.Failure<IEnumerable<QuestionResponse>>(PollErrors.PollNotFound);
            var cachekey = $"{_QuestionscachePrefix}_{pollId}";
            var cacheQuestions = await _distributedCache.GetTAsync<IEnumerable<QuestionResponse>>(cachekey, cancellationToken);
            if (cacheQuestions is null)
            {
                cacheQuestions = await _AppDbContext.Questions.Where(x => x.PollId == pollId).Select(
                q => new QuestionResponse
                   {
                       Id = q.Id,
                       Contant = q.Content,
                       AnswerResponses = q.Answers.Select(a => new AnswerResponse { Id = a.Id, Contant = a.Content ,IsActive = a.IsActive })
                   }
                ).AsNoTracking()
               .ToListAsync();
                await _distributedCache.SetAsync(cachekey, cacheQuestions, cancellationToken);
            }   
            return Result<IEnumerable<QuestionResponse>>.Success(cacheQuestions!);
        }

        public async Task<Result<QuestionResponse>> GetAsync(int pollId, int questionId, CancellationToken cancellationToken = default)
        {
            var IsPollExist = await _AppDbContext.Questions.AnyAsync(x => x.PollId == pollId && x.Id == questionId);
            if(!IsPollExist)
                return Result<QuestionResponse>.Failure<QuestionResponse>(QuestionErrors.QuestionNotFound);
            var Questions = await _AppDbContext.Questions.FirstOrDefaultAsync(x => x.Id == questionId);
            if (Questions is null)
                return Result<QuestionResponse>.Failure<QuestionResponse>(QuestionErrors.InternalServerError);
            QuestionResponse questionResponse = new QuestionResponse
            {
                Id = Questions.Id,
                Contant = Questions.Content,
                AnswerResponses = _AppDbContext.Answers.Where(a => a.QuestionId == Questions.Id).Select(a => new AnswerResponse
                {
                    Contant = a.Content,
                    Id = a.Id,
                    IsActive = a.IsActive
                })
            };
            return Result<QuestionResponse>.Success(questionResponse);
        }

        public async Task<Result> UpdateAsync(QuestionRequest request, int pollId, int id, CancellationToken cancellationToken = default)
        {
            var IsExistquestion = await _AppDbContext.Questions.AnyAsync(x => x.PollId == pollId  && x.Content == request.Contant && x.Id != id);
            if (IsExistquestion)
                return Result.Failure(QuestionErrors.DuplicatedQuestionContent);
            var question = await _AppDbContext.Questions.Include(x => x.Answers).SingleOrDefaultAsync(x => x.PollId == pollId && x.Id == id);
            if (question is null)
                return Result.Failure(QuestionErrors.QuestionNotFound);
            question.Content = request.Contant;
            var answers = question.Answers.Select(x => x.Content).ToList();
            var newanswers = request.Answers.Except(answers).ToList();
            newanswers.ForEach(x =>
            {
                question.Answers.Add(new Answer { Content = x });
            });
            question.Answers.ToList().ForEach(x =>
            {
                x.IsActive = request.Answers.Contains(x.Content);
            });
            _AppDbContext.Questions.Update(question);
            var rows =  await _AppDbContext.SaveChangesAsync();
            if(rows > 0)
            {
                var cachekey = $"{_QuestionscachePrefix}_{pollId}";
                await _distributedCache.RemoveAsync(cachekey);
            }
            return rows > 0?  Result.Success() : Result.Failure(QuestionErrors.InternalServerError);
        }


        public async Task<Result<IEnumerable<QuestionResponse>>> AvalibleQuestionsAsync(int pollId, string userId, CancellationToken cancellationToken = default)
        {
            var polls = await _AppDbContext.polls
              .AnyAsync(x => x.Id == pollId && x.IsPublished && x.EndAt > DateOnly.FromDateTime(DateTime.UtcNow) && x.StartAt <= DateOnly.FromDateTime(DateTime.UtcNow));
            if (!polls)
                return Result<IEnumerable<QuestionResponse>>.Failure<IEnumerable<QuestionResponse>>(QuestionErrors.QuestionNotFound);
            var QuestionsIdx = await (from va in _AppDbContext.VoteAnswers                   
                               join v in _AppDbContext.Votes
                               on va.VoteId equals v.Id
                               where v.UserId == userId  && v.PollId == pollId    
                               select va.QuestionId
                               ).ToListAsync();
            var Questions = await _AppDbContext.Questions
                .Where(x => x.PollId == pollId && x.IsActive && !QuestionsIdx.Contains(x.Id)).Include(x => x.Answers)
                .Select(q => new QuestionResponse
                {
                    Id = q.Id,
                    Contant = q.Content,
                    AnswerResponses = q.Answers.Where(a => a.IsActive).Select(a => new AnswerResponse { Id = a.Id, Contant = a.Content })

                }).AsNoTracking()
                .ToListAsync(cancellationToken);

            return Result<IEnumerable<QuestionResponse>>.Success<IEnumerable<QuestionResponse>>(Questions);

        }



        public async Task<Result> ToggleStatusAsync(int pollId, int id, CancellationToken cancellationToken = default)
        {
            var question = await _AppDbContext.Questions.SingleOrDefaultAsync(x => x.Id == id && x.PollId == pollId);
            if (question is null)
                return Result.Failure(QuestionErrors.QuestionNotFound);
            question.IsActive = !question.IsActive;
            await _AppDbContext.SaveChangesAsync();
            return Result<QuestionResponse>.Success(question.Adapt<QuestionResponse>());
        }

        
    }


}