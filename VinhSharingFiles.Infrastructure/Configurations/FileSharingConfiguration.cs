using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VinhSharingFiles.Domain.Entities;

namespace VinhSharingFiles.Infrastructure.Configurations
{
    public class FileSharingConfiguration : IEntityTypeConfiguration<FileSharing>
    {
        public void Configure(EntityTypeBuilder<FileSharing> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.FileName)
                .HasMaxLength(250)
                .IsRequired();
            builder.Property(x => x.FilePath)
                .HasMaxLength(500)
                .IsRequired();
            builder.Property(x => x.FileType)
                .HasMaxLength(100)
                .IsRequired();
            builder.Property(x => x.UserId)
                .IsRequired();
        }
    }
}
