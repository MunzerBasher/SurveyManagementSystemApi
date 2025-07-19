namespace SurveyBasket.Services;

public class NotificationService(
    AppDbContext context,
    UserManager<UserIdentity> userManager,
    IHttpContextAccessor httpContextAccessor,
    IEmailSender emailSender) : INotificationService
{
    private readonly AppDbContext _context = context;
    private readonly UserManager<UserIdentity> _userManager = userManager;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IEmailSender _emailSender = emailSender;

    public async Task SendNewPollsNotification(int? pollId = null)
    {
        IEnumerable<Poll> polls = [];

        if (pollId.HasValue)
        {
            var poll = await _context.polls.SingleOrDefaultAsync(x => x.Id == pollId && x.IsPublished);

            polls = [poll!];
        }
        else
        {
            polls = await _context.polls
                .Where(x => x.IsPublished && x.StartAt == DateOnly.FromDateTime(DateTime.UtcNow))
                .AsNoTracking()
                .ToListAsync();
        }

        var users = await _userManager.GetUsersInRoleAsync(DefaultRoles.Member);

        var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;

        foreach (var poll in polls)
        {
            foreach (var user in users)
            {
                var placeholders = new Dictionary<string, string>
                {
                    { "{{name}}", user.FirstName + "  " +  user.LastName},
                    { "{{pollTill}}", poll.Title },
                    { "{{endDate}}", poll.EndAt.ToString() },
                    { "{{url}}", $"{origin}/polls/start/{poll.Id}" }
                };

                var body = EmailBodyBuilder.GenerateEmailBody("PollNotification", placeholders);

                await _emailSender.SendEmailAsync(user.Email!, $"📣 Survey Basket: New Poll - {poll.Title}", body);
            }
        }
    }

}