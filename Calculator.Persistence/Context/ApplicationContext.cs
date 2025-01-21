using Calculator.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SmartEnum.EFCore;

namespace Calculator.Persistence.Context;

public class ApplicationContext : DbContext
{
    private readonly IConfiguration _configuration;

    public DbSet<User> Users { get; set; }

    public ApplicationContext(IConfiguration configuration)
    {
        _configuration = configuration;

        // only for demonstration - not for production use
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseSqlServer(_configuration.GetConnectionString("ApplicationContext"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(user =>
        {
            user.HasKey(user => user.Id);
            user.HasIndex(user => user.InternetAddress); // not always true but for the example it is
            user.OwnsMany(p => p.Operations);
        });

    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.ConfigureSmartEnum();
    }

}
