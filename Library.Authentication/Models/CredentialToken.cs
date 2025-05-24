namespace Library.Authentication.Models;

public record CredentialToken(string Salt, string Vector, string EncryptedToken);