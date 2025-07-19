using SurveyManagementSystemApi.Contracts.Vote;

namespace SurveyManagementSystemApi.Controllers
{
    [Route("api/polls/{pollId}/Vote")]
    [ApiController]
    [Authorize]
    public class VoteController(IQuestion question, IVoteServices voteServices) : ControllerBase
    {
        private readonly IQuestion _question = question;

        public IVoteServices _voteServices { get; } = voteServices;
        [HttpGet("")]
        public async Task<ActionResult<Result<IEnumerable<QuestionResponse>>>> AvalibleQuestions([FromRoute]int pollId)
        {
            var userId = User.GetUserId()!;
            var response = await _question.AvalibleQuestionsAsync(pollId, userId);    
            return response.IsSuccess ? Ok(response.Value) : response.ToProblem();
        }

        [HttpPost("")]
        public async Task <ActionResult<Result>> Add([FromRoute] int pollId, [FromBody] VoteRequest voteRequest, CancellationToken cancellationToken = default)
        {
            var userId = User.GetUserId()!;
            var result = await _voteServices.AddAsync(pollId, userId, voteRequest, cancellationToken);
            return result.IsSuccess ? Ok() : result.ToProblem();
        }



    } 
}