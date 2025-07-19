namespace SurveyManagementSystemApi.Entities
{
    [Owned]
    public class RefreshToken
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresOn { get; set; }
        public DateTime CreateOn { get; set; }
        public DateTime? RefreshOn {  get; set; }
        public bool IsExpires => ExpiresOn >= DateTime.Now;
        public bool IsActive => RefreshOn is null && !IsExpires;
       
    }
}
