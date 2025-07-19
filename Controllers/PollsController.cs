namespace SurveyManagementSystemApi.Controllers;


[Route("api/[controller]")]
[ApiController]
[Authorize]

public class PollsController(IPollService pollService) : ControllerBase
{
    private readonly IPollService _pollService = pollService;
    //[HasPermission(Permissions.GetPolls)]
    [HttpGet("")]
    public async Task<ActionResult<Result<IEnumerable<PollResponse>>>> GetAll(CancellationToken cancellationToken = default)
    {
        var polls = await _pollService.GetAllAsync();
        return Ok(polls.Value);
    }

    [HttpGet("Current")]
    public async Task<ActionResult<Result<IEnumerable<PollResponse>>>> GetCurrent(CancellationToken cancellationToken = default)
    {
        var polls = await _pollService.GetCurrentAsync();
        return polls.IsSuccess ? Ok(polls.Value) : polls.ToProblem();
    }
  
    [HttpGet("{id}")]
    
    public async Task<ActionResult<Poll>> Get(int id, CancellationToken cancellationToken = default)
    {
        var result = await _pollService.GetByIdAsync(id)!;
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<bool>> Delete(int id, CancellationToken cancellationToken = default)
    {
        var result = await _pollService.DeleteAsync(id);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    
    [HttpPut("{id}")]
    public async Task<ActionResult<Result<Poll>>> Update(int id, [FromBody] PollRequest poll, CancellationToken cancellationToken = default)
    {
        var result = await _pollService.UpdateAsync(id, poll);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }


    [HttpPost("")]
    public async Task<IActionResult> Add([FromBody] PollRequest poll, CancellationToken cancellationToken = default)
    {
        var result = await _pollService.AddAsync(poll);
        return result.IsSuccess ? Ok() : result.ToProblem();

    }

    [HttpPost("ToggleStatus/{pollId}")]
    public async Task<IActionResult> ToggleStatus([FromRoute]int pollId, CancellationToken cancellationToken = default)
    {
        var result = await _pollService.ToggleStatus(pollId, cancellationToken);
        return result.IsSuccess ? Ok() : result.ToProblem();
    }





}