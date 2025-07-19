namespace SurveyManagementSystemApi.Configurations
{
    
    public class VoteAnswerConfigurations : IEntityTypeConfiguration<VoteAnswers>
    {
        public void Configure(EntityTypeBuilder<VoteAnswers> builder)
        {
            builder.HasIndex(x => new { x.AnswerId, x.QuestionId ,x.VoteId}).IsUnique();

        }
    }


}
