namespace SurveyManagementSystemApi.Controllers
{

    [Route("api/Auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IAuthService _loginService;
 
        public AuthController(IAuthService loginService)
        {
            _loginService = loginService;
            
        }


        [HttpPost("Login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest loginRequest)
        {
           
            var response = await _loginService.LoginAsync(loginRequest);
            return response.IsSuccess ? Ok(response.Value) : response.ToProblem();
        }
        
        [HttpPost("Register")]
        public async Task<ActionResult<string>> Register([FromBody] RegisterRequest registerRequest)
        {
            var result = await _loginService.Register(registerRequest); 
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        [HttpPost("ReFreshToken")]
        public async Task<ActionResult<AuthResponse>> ReFreshToken([FromBody] ReFreshTokenRequest refreshTokenRequest, CancellationToken cancellationToken)
        {
            var response = await _loginService.GetReFreshTokenAsnyc(refreshTokenRequest,cancellationToken);
            return response.IsSuccess ? Ok(response.Value) : response.ToProblem();
        }


        [HttpPost("Conform-Email")]
        public async Task<IActionResult> ConformEmailAsync(ConfirmEmailRequest request)
        {
            var response = await _loginService.ConformEmailAsync(request);
            return response.IsSuccess ? Ok() : response.ToProblem();    
        }

        
        [HttpPost("ResendConform-Email")]
        public async Task<IActionResult> ResendConfirmationEmailAsync(ResendConfirmationEmailRequest request, CancellationToken cancellationToken = default)
        {
            var response = await _loginService.ResendConfirmationEmailAsync(request, cancellationToken);
            return response.IsSuccess ? Ok() : response.ToProblem();
        }

        [HttpPost("Forget-Password")]
        public async Task<IActionResult> ForgetPasswordAsync(string emali)
        {
            var response = await _loginService.SendResetPasswordCodeAsync(emali);
            return response.IsSuccess ? Ok() : response.ToProblem();
        }

        [HttpPost("Reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var result = await _loginService.ResetPasswordAsync(request);

            return result.IsSuccess ? Ok() : result.ToProblem();
        }
    }

}