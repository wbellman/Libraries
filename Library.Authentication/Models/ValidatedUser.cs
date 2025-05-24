namespace Library.Authentication.Models;

public record ValidatedUser(
    string Username,
    string Email,
    string Token
);