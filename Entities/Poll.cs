namespace SurveyManagementSystemApi.Entities
{
    public sealed class Poll : AuditableEntity
    {
        public int Id { get; set; }
        public required string  Title { get; set; }
        public required string Description { get; set; }  
        public bool IsPublished { get; set; }   
        public DateOnly StartAt { get; set; }
        public DateOnly EndAt { get; set; }
        public ICollection<Question> Questions { get; set; } = [];
        public ICollection<Vote> Votes { get; set ; } = [];
    }
}
