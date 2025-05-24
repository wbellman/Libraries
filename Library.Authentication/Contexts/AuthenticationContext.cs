using Library.Authentication.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.Authentication.Contexts;

public class AuthenticationContext(
    DbContextOptions<AuthenticationContext> options
) : DbContext(options)
{
    public DbSet<Credential> Credentials { get; set; }
    public DbSet<User> Users { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}