namespace SurveyManagementSystemApi.IServices
{
    public interface IUsersService
    {
        public Task<Result<UserProfileResponse>> GetProfileAsync(string UserId);


        public Task<Result> UpdateProfileAsync(UpdateProfileRequest updateProfileRequest, string userId);


        public Task<Result> ChangePasswordAsync(ChangePasswordRequest changePasswordRequest, string userId);


    }
}

