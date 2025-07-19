namespace SurveyManagementSystemApi.Services
{
    public class UsersService(UserManager<UserIdentity> userManager) : IUsersService
    {
        private readonly UserManager<UserIdentity> _userManager = userManager;



        public async Task<Result> ChangePasswordAsync(ChangePasswordRequest request, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            var result = await _userManager.ChangePasswordAsync(user!, request.OldPasswor, request.NewPasswor);

            if (result.Succeeded)
                return Result.Success();

            var error = result.Errors.First();

            return Result.Failure(new Error(error.Code,StatusCodes.Status400BadRequest));

        }

        public async Task<Result<UserProfileResponse>> GetProfileAsync(string userId)
        {
            var  user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId);

            return Result<UserProfileResponse>.Success(new UserProfileResponse
            { 
                Email = user!.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName!
            
            });
        }

        public async Task<Result> UpdateProfileAsync(UpdateProfileRequest updateProfileRequest,string userId)
        {
            await _userManager.Users
           .Where(x => x.Id == userId)
           .ExecuteUpdateAsync(setters =>
               setters
                   .SetProperty(x => x.FirstName, updateProfileRequest.FirstName)
                   .SetProperty(x => x.LastName, updateProfileRequest.LastName)
           );

            return Result.Success();
        }

    }

}