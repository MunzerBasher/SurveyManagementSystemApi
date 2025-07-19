namespace SurveyManagementSystemApi.Entities
{
    public class UserRoles : IdentityRole
    {
        public bool IsDefault { get; set; }
        public bool IsDeleted { get; set; } 
    }
}
