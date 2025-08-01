﻿
namespace SurveyManagementSystemApi.Configurations
{
    public class QuestionConfiguration : IEntityTypeConfiguration<Question>
    {
        public void Configure(EntityTypeBuilder<Question> builder)
        {
            builder.HasIndex(x => new { x.Content, x.PollId }).IsUnique();
            builder.Property(x => x.Content).HasMaxLength(500).IsRequired();
            builder.HasKey(x => x.Id);
        }
    }
}
