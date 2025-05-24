using Library.Authentication.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.Authentication.Configurations;

public class Users : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .ValueGeneratedOnAdd();
        
        builder.Property(c => c.Username)
            .IsRequired();
        
        builder.Property(c => c.Email)
            .IsRequired();
    }
}