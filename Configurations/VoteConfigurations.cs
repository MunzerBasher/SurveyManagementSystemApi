namespace SurveyManagementSystemApi.Configurations
{
    public class VoteConfigurations : IEntityTypeConfiguration<Vote>
    {
        public void Configure(EntityTypeBuilder<Vote> builder)
        {
            builder.HasIndex(x => new { x.UserId, x.PollId }).IsUnique();
           
        }
    }
}
