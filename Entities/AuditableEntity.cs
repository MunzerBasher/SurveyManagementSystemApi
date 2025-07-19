namespace SurveyManagementSystemApi.Entities
{
    public class AuditableEntity
    {
        public string CreatedId { get; set; } = string.Empty;
        public string UpdatedId { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedOn { get; set; }    
    }

}
