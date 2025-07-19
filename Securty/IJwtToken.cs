namespace SurveyManagementSystemApi.Securty
{
    public interface IJwtToken
    {
        public string GenerateToken(UserIdentity user, IEnumerable<string> roles, IEnumerable<string> permissions);

        string? ValidateToken(string token);


    }

}
