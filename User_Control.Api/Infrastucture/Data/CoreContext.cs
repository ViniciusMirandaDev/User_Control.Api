using Microsoft.EntityFrameworkCore;
using User_Control.Api.Application.Entities;
using User_Control.Api.Infrastucture.Data.EntityConfigurations;

namespace User_Control.Api.Infrastucture.Data
{
    public class CoreContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public CoreContext(DbContextOptions<CoreContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new UserEntityTypeConfiguration());
            base.OnModelCreating(builder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
        }
    }
}
