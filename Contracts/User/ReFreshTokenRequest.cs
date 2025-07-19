namespace SurveyManagementSystemApi.Contracts.User
{
    public class ReFreshTokenRequest
    {

        public string token {  get; set; } = string.Empty;

        public string rFreshToken { get; set; }= string.Empty;
    }
}
