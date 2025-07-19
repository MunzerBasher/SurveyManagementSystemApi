using SurveyManagementSystemApi.Abstractions.Consts;

namespace SurveyManagementSystemApi.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<UserIdentity>
    {
        public void Configure(EntityTypeBuilder<UserIdentity> builder)
        {
            builder.Property(x => x.FirstName).HasMaxLength(50);
            builder.Property(x =>x.LastName).HasMaxLength(50);
            builder.OwnsMany(x => x.RefreshTokens).ToTable("RefreshTokens").WithOwner().HasForeignKey("UseId");


            var passwordHasher = new PasswordHasher<UserIdentity>();

            builder.HasData(new UserIdentity
            {
                Id = DefaultUsers.AdminId,
                FirstName = "Survey Basket",
                LastName = "Admin",
                UserName = DefaultUsers.AdminEmail,
                NormalizedUserName = DefaultUsers.AdminEmail.ToUpper(),
                Email = DefaultUsers.AdminEmail,
                NormalizedEmail = DefaultUsers.AdminEmail.ToUpper(),
                SecurityStamp = DefaultUsers.AdminSecurityStamp,
                ConcurrencyStamp = DefaultUsers.AdminConcurrencyStamp,
                EmailConfirmed = true,
                PasswordHash = passwordHasher.HashPassword(null!, DefaultUsers.AdminPassword)
            });
        }
    }
}
