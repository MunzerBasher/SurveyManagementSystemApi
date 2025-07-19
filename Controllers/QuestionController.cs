using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace SurveyManagementSystemApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class QuestionController(IQuestion question) : ControllerBase
    {
        private readonly IQuestion _question = question;

        [HttpGet("{PollId}")]
        public async Task<ActionResult<Result<IEnumerable<QuestionResponse>>>> GetAll(int PollId)
        {
            var Questions = await _question.GetAllAsync(PollId);

            return Questions.IsSuccess ? Ok(Questions.Value) : Questions.ToProblem();
        }

      [EnableRateLimiting("IpLimiter")]
        [HttpGet("AvalibleQuestions/{PollId}")]
        public async Task<ActionResult<Result<IEnumerable<QuestionResponse>>>> GetAvalibleQuestions(int PollId)
        {
            var Questions = await _question.AvalibleQuestionsAsync(PollId,User.GetUserId()!);

            return Questions.IsSuccess ? Ok(Questions.Value) : Questions.ToProblem();
        }

        [HttpGet("{pollID}/{questionId}")]
        public async Task<ActionResult<Result<QuestionResponse>>> Get(int pollID,int questionId)
        {
            var Questions = await _question.GetAsync(pollID, questionId);

            return Questions.IsSuccess ? Ok(Questions.Value) : Questions.ToProblem();
        }


        [HttpPost("")]
        public async Task<ActionResult<Result<QuestionResponse>>> Add([FromBody]QuestionRequest Request)
        {
            var Questions = await _question.AddAsync(Request);
            return Questions.IsSuccess ? Created() : Questions.ToProblem();
        }

        [HttpPut("")]
        public async Task<ActionResult<Result<QuestionResponse>>>  Update(QuestionRequest Request,int pollId,int id)
        {
            var response = await _question.UpdateAsync(Request,pollId,id);
            return response.IsSuccess? Ok(response) : response.ToProblem();
        }

        [HttpPost("/{pollId}ToggleStatus{questionId}")]
        public async Task<ActionResult<Result<QuestionResponse>>> ToggleStatus( int pollId, int questionId)
        {
            var response = await _question.ToggleStatusAsync(pollId, questionId);
            return response.IsSuccess ? Ok(response) : response.ToProblem();
        }

    }



}