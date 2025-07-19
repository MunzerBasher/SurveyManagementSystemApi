
namespace SurveyManagementSystemApi.Configurations
{
    public class pollConf : IEntityTypeConfiguration<Poll>
    {
        public void Configure(EntityTypeBuilder<Poll> builder)
        {
            builder.Property(x => x.Title).IsRequired();
            builder.HasIndex(x => x.Title).IsUnique();
            builder.HasKey(x => x.Id);
        }
    }
}
