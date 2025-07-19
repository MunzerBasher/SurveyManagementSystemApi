

namespace SurveyManagementSystemApi.Configurations
{

    public class AnswerConf : IEntityTypeConfiguration<Answer>
    {
        public void Configure(EntityTypeBuilder<Answer> builder)
        {
            builder.HasIndex( x => new { x.Content , x.QuestionId}).IsUnique();
            builder.Property(x => x.Content).HasMaxLength(500);
            builder.HasKey(x => x.Id);
        }
    }

}
