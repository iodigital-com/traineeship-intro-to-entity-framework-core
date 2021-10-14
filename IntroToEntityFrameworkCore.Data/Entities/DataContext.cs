using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace IntroToEntityFrameworkCore.Data.Entities;
public class DataContext : DbContext
{
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Commute> Commutes => Set<Commute>();


    private readonly string connectionString = string.Empty;
    
    public DataContext() : base()
    {
        // this constructor is solely for dotnet ef migrations command
    }
    
    public DataContext(DbContextOptions<DataContext> options)
        : base(options)
    {
    }
        
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseSqlServer(connectionString,
            x =>
            {
                x.UseNetTopologySuite();
            });
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasIndex(x => x.Name);
            entity
                .HasIndex(x => x.Email)
                .IsUnique();
        });
    }
    
    // Previous versions of EF Core require that the mapping for every property of a given type is configured explicitly when that mapping differs from the default.
    // This includes "facets" like the maximum length of strings and decimal precision, as well as value conversion for the property type.
    // EF Core 6.0 allows this mapping configuration to be specified once for a given type. It will then be applied to all properties of that type in the model.
    // This is called "pre-convention model configuration", since it configures aspects of the model that are then used by the model building conventions. 
    // https://docs.microsoft.com/en-us/ef/core/what-is-new/ef-core-6.0/whatsnew#pre-convention-model-configuration
    protected override void ConfigureConventions(
        ModelConfigurationBuilder configurationBuilder)
    {
        
        // Limit all string properties to have a maximum length of 128 characters
        configurationBuilder.Properties<string>().HaveMaxLength(128);
    }
    
    /// <summary>
    /// Automatically set CreatedAt and/or UpdatedAt fields on saving changes
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is BaseEntity && e.State is EntityState.Added or EntityState.Modified);

        foreach (var entityEntry in entries)
        {
            ((BaseEntity)entityEntry.Entity).UpdatedAt = DateTime.UtcNow;

            if (entityEntry.State == EntityState.Added)
            {
                ((BaseEntity)entityEntry.Entity).CreatedAt = DateTime.UtcNow;
            }
        }
        return await base.SaveChangesAsync(true, cancellationToken);
    }
}