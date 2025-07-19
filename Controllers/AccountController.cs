namespace SurveyManagementSystemApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController(IUsersService usersService) : ControllerBase
    {


        private readonly IUsersService _usersService = usersService;


        [HttpGet("Profile")]
        public async Task<ActionResult<UserProfileResponse>> GetProfile()
        {
            var profile = await _usersService.GetProfileAsync(User.GetUserId()!);

            return Ok(profile.Value);
        }

        [HttpPut("Profile")]
        public async Task<IActionResult> UpdateProfile(UpdateProfileRequest updateProfileRequest)
        {
            var profile = await _usersService.UpdateProfileAsync(updateProfileRequest, User.GetUserId()!);
            return Ok(profile);
        }


        [HttpPut("Change-Password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest changePasswordRequest)
        {
            var profile = await _usersService.ChangePasswordAsync(changePasswordRequest, User.GetUserId()!);   
            return Ok(profile);
        }



    }
}