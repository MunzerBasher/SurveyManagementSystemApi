namespace SurveyManagementSystemApi.Persistence
{



    public class AppDbContext(DbContextOptions<AppDbContext> options, IHttpContextAccessor httpContextAccessor) :
        IdentityDbContext<UserIdentity, UserRoles,string>(options)
    {
        public DbSet<Poll> polls { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Vote> Votes { get; set; }
        public DbSet<VoteAnswers> VoteAnswers { get; set; }


        private readonly IHttpContextAccessor _HttpContextAccessor = httpContextAccessor;
    
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());


            var cascaddefks = modelBuilder.Model.GetEntityTypes().SelectMany(t => t.GetForeignKeys()).
                Where(fk => fk.DeleteBehavior ==DeleteBehavior.Cascade && !fk.IsOwnership);
            foreach ( var c in cascaddefks )
                c.DeleteBehavior = DeleteBehavior.Restrict;

            base.OnModelCreating(modelBuilder);
        }

       
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            var entities = ChangeTracker.Entries<AuditableEntity>();
            foreach (var entity in entities)
            {
                var UserId = _HttpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (entity.State == EntityState.Modified)
                {
                    entity.Property(x => x.UpdatedOn).CurrentValue = DateTime.UtcNow;
                    entity.Property(x => x.UpdatedId).CurrentValue = UserId;
                }
                else if (entity.State == EntityState.Added)
                {
                    entity.Property(x => x.UpdatedOn).CurrentValue = DateTime.UtcNow;
                    entity.Property(x => x.CreatedId).CurrentValue = UserId;
                }
            }
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }


}
