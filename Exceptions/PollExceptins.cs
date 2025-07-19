namespace SurveyManagementSystemApi.Exceptions
{
    public class PollExceptins(ILogger<PollExceptins> logger, RequestDelegate requestDelegate)
    {
        private readonly ILogger<PollExceptins> _logger = logger;
        private readonly RequestDelegate _requestDelegate = requestDelegate;

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            try
            {
                await _requestDelegate(httpContext);
            }
            catch
            {
                _logger.LogError(exception, message :  exception.Message, "Some thing happen");
                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = exception.Message,
                    Type = " "
                };
                await httpContext.Response.WriteAsJsonAsync(problemDetails,CancellationToken.None);
            }
           return true;
        }
    }
}