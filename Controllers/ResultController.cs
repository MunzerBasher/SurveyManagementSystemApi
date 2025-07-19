using SurveyManagementSystemApi.Contracts.Vote;

namespace SurveyManagementSystemApi.Controllers
{


    [Route("api/Result/{PollId}")]
    [ApiController]
    [Authorize]
    public class ResultController(IResultServices resultServices) : ControllerBase
    {
        private readonly IResultServices _resultServices = resultServices;

        [HttpGet("")]
        public async Task<ActionResult<PollVoteResponse>> GetPollResult([FromRoute] int PollId)
        {
            var result = await _resultServices.PollResult(PollId);
            return result.IsSuccess ? Ok(result) : result.ToProblem();
        }

        [HttpGet("PerDay")]
        public async Task<ActionResult<VotePerDayResponse>> GetPollResultPerDay([FromRoute] int PollId)
        {
            var result = await _resultServices.GetVotePerDay(PollId);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        [HttpGet("PerQuestion")]
        public async Task<ActionResult<VotePerQuestionResponse>> GetPollResultPerQuestion(int PollId)
        {
            var result = await _resultServices.GetVotePerQuestion(PollId);
            return result.IsSuccess ? Ok(result) : result.ToProblem();
        }

    }


}