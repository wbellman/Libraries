namespace Library.Authentication.Models;

public static class Requests
{
    public static class Users
    {
        public record Create(string Username, string Email, string Password);

        public record Login(string Username, string Password);
    }
}