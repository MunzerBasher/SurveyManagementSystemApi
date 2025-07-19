namespace SurveyManagementSystemApi.Entities
{
    public class UserIdentity : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public List<RefreshToken> RefreshTokens { get; set; } = [];
        public bool IsDisabled { get; set; } = false;

    }
}
