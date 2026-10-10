namespace Taxi.Core.Models.Auth
{
    public record RegisterRequest(string login, string password, string lastName,
        string firstName, string middleName, string email);
}
