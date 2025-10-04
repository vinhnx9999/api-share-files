using Microsoft.EntityFrameworkCore;
using VinhSharingFiles.Domain.Entities;
using VinhSharingFiles.Infrastructure.Configurations;

namespace VinhSharingFiles.Infrastructure.Data;

public class VinhSharingDbContext(DbContextOptions<VinhSharingDbContext> options) : DbContext(options)
{

    //All these are tables which are entities in domain layer. this way we can access the database tables
    public DbSet<User> Users { get; set; }
    public DbSet<FileSharing> FileSharings { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new FileSharingConfiguration());
        base.OnModelCreating(modelBuilder); // Call to the base method
    }
}
