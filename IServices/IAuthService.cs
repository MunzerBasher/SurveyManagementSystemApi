namespace SurveyManagementSystemApi.IServices
{
    public interface IAuthService
    {

        Task<Result<AuthResponse>> LoginAsync(LoginRequest loginRequest, CancellationToken cancellationToken = default);


        Task<Result<AuthResponse>> GetReFreshTokenAsnyc(ReFreshTokenRequest refreshTokenRequest, CancellationToken cancellationToken);


        Task<Result<string>> Register(RegisterRequest registerRequest, CancellationToken cancellationToken = default);


        Task<Result> ConformEmailAsync(ConfirmEmailRequest request, CancellationToken cancellationToken = default);


        Task<Result> ResendConfirmationEmailAsync(ResendConfirmationEmailRequest request, CancellationToken cancellationToken = default);

        

        Task<Result> SendResetPasswordCodeAsync(string email);



        Task<Result> ResetPasswordAsync(ResetPasswordRequest request);
    }

}