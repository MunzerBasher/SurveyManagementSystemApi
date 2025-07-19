namespace SurveyManagementSystemApi.Entities
{
    public record AuthResponse
    {
        string Id { get; set; }
        string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Token { get; set; }
        public int ExpiresIn { get; set; }
        public string RefreshToken  {get; set; }
        DateTime RefreshTokenExpires { get; set; }

    }


}
