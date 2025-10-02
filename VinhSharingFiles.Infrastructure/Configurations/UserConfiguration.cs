using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VinhSharingFiles.Domain.Entities;

namespace VinhSharingFiles.Infrastructure.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserName)
                .HasMaxLength(50);

            builder.Property(x => x.Email)
                .HasMaxLength(250)
                .IsRequired(); 

            builder.HasData(
                new User { Id = 1, IsActive = true, DisplayName = "User 01", UserName = "vinh.nguyen", Email = "vinhnx9999@gmail.com", Password = "123456", CreatedAt = DateTime.Now },
                new User { Id = 2, IsActive = true, DisplayName = "User 02", UserName = "vinh.xuan", Email = "xuanvinh9999@gmail.com", Password = "123456", CreatedAt = DateTime.Now }
            );
        }
    }
}
