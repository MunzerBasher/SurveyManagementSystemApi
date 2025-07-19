namespace SurveyManagementSystemApi.Service
{


    public class PollService : IPollService
    {
        public PollService(AppDbContext appDbContext, ICacheService distributedCache, INotificationService notificationService)
        {
            _appDbContext = appDbContext;
            _distributedCache = distributedCache;
            _notificationService = notificationService;
        }

 
        private readonly AppDbContext _appDbContext;
        private readonly ICacheService _distributedCache;
        private readonly INotificationService _notificationService;
        private const string _PollscachePrefix = "Pollshw53#22";
        private const string CurrentPollscachekey = $"{_PollscachePrefix}Current";

        public async Task<Result> AddAsync(PollRequest poll, CancellationToken cancellationToken = default)
        {
            if( await _appDbContext.polls.AnyAsync(x => x.Title == poll.Title))
                return Result.Failure(PollErrors.DuplicatedPollTitle);
            await _appDbContext.polls.AddAsync(poll.Adapt<Poll>());
            var isAdded =  await _appDbContext.SaveChangesAsync();
            if (isAdded > 0)
                await _distributedCache.RemoveAsync(CurrentPollscachekey);
            return isAdded > 0 ? Result.Success() : Result.Failure(PollErrors.InternalServerError);
        }

        public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            if (id < 1)
                return Result.Failure(PollErrors.PollBadRequest);
            var IsExist = await _appDbContext.polls.FindAsync(id);
            if (IsExist is null)
                Result.Failure(PollErrors.PollNotFound);
             _appDbContext.polls.Remove(IsExist!);
              var rows = await _appDbContext.SaveChangesAsync();
            if(rows > 0)
            {
                await _distributedCache.RemoveAsync(CurrentPollscachekey);
                return Result.Success();
            }
            return Result.Failure(PollErrors.InternalServerError);
        }

        public async Task<Result<IEnumerable<PollResponse>>> GetAllAsync( CancellationToken cancellationToken = default)
        {
            var polls =  await _appDbContext.polls.ProjectToType<PollResponse>().AsNoTracking().ToListAsync();
            return Result<IEnumerable<PollResponse>>.Success<IEnumerable<PollResponse>>(polls);
        }

        public async Task<Result<IEnumerable<PollResponse>>> GetCurrentAsync(CancellationToken cancellationToken = default)
        {
            
            var Currentpolls = await _distributedCache.GetTAsync<IEnumerable<PollResponse>>(CurrentPollscachekey, cancellationToken);
            if(Currentpolls is null)
            {
                Currentpolls  = await _appDbContext.polls
                .Where(x => x.IsPublished && x.EndAt > DateOnly.FromDateTime(DateTime.UtcNow) && x.StartAt <= DateOnly.FromDateTime(DateTime.UtcNow))
                .ProjectToType<PollResponse>().AsNoTracking().ToListAsync();
                 await _distributedCache.SetAsync(CurrentPollscachekey, Currentpolls, cancellationToken);
            } 
            return Result<IEnumerable<PollResponse>>.Success(Currentpolls);
        }

        public async Task<Result<Poll>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            if (id < 1)
                return Result<Poll>.Failure<Poll>(PollErrors.PollBadRequest);
            var poll = await _appDbContext.polls.FindAsync(id); 
            return poll is null ? Result<Poll>.Failure<Poll>(PollErrors.PollNotFound):
                Result<Poll>.Success(poll);
        }


        public async Task<Result<Poll>> UpdateAsync(int id, PollRequest poll, CancellationToken cancellationToken = default)
        {
            if (id < 1)
                return Result<Poll>.Failure<Poll>(PollErrors.PollBadRequest);
            if (await _appDbContext.polls.AnyAsync(x => x.Title == poll.Title && x.Id != id))
                return Result<Poll>.Failure<Poll>(PollErrors.DuplicatedPollTitle);
            var updated = await _appDbContext.polls.FindAsync(id);
            if (updated != null)
            {
                updated.Description = poll.Description;
                updated.Title = poll.Title;
                var rows = await _appDbContext.SaveChangesAsync();
                if (rows > 0)
                    await _distributedCache.RemoveAsync(CurrentPollscachekey);
                return rows < 1 ? Result<Poll>.Failure<Poll>(PollErrors.InternalServerError) :
                    Result<Poll>.Success(updated);
            }
            return Result<Poll>.Failure<Poll>(PollErrors.PollNotFound)!;
        
        }

        public async Task<Result> ToggleStatus(int pollId , CancellationToken cancellationToken = default)
        {
            if (pollId < 1)
                return Result.Failure(PollErrors.PollBadRequest);
            var updated = await _appDbContext.polls.FindAsync(pollId);
            if (updated != null)
            {
                var IsPublished = updated.IsPublished;
                updated.IsPublished = !IsPublished;
                var rows = await _appDbContext.SaveChangesAsync();
                if (rows > 0)
                {
                    await _distributedCache.RemoveAsync(CurrentPollscachekey);
                    await _notificationService.SendNewPollsNotification(pollId);
                }
                return rows < 1 ? Result.Failure(PollErrors.InternalServerError) :
                    Result.Success();
            }
            return Result.Failure(PollErrors.PollNotFound)!;
        }

        
    }

}