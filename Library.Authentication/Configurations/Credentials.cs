using Library.Authentication.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.Authentication.Configurations;

public class Credentials : IEntityTypeConfiguration<Credential>
{
    public void Configure(EntityTypeBuilder<Credential> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .ValueGeneratedOnAdd();

        builder.Property(c => c.UserId)
            .IsRequired();

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.ExpirationDate)
            .IsRequired(false);

        builder.Property(c => c.IsValid)
            .HasDefaultValue(true)
            .IsRequired();

        builder.OwnsOne(c => c.Token, token =>
        {
            token.Property(t => t.Salt)
                .HasMaxLength(64)
                .IsRequired();

            token.Property(t => t.Vector)
                .HasMaxLength(64)
                .IsRequired();

            token.Property(t => t.EncryptedToken)
                .HasMaxLength(512)
                .IsRequired();
        });

        builder.HasIndex(c => c.UserId).IsUnique(false);
    }
}