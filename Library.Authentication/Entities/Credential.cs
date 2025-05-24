using Library.Authentication.Models;

namespace Library.Authentication.Entities;

public class Credential
{
    public Guid? Id { get; set; }
    public Guid? UserId { get; set; } 

    public CredentialToken? Token { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ExpirationDate { get; set; }

    public bool IsValid { get; set; } = true;
}