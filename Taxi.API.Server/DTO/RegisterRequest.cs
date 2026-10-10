namespace Taxi.API.Server.DTO
{
    public record RegisterRequest(string login, string password, string lastName, string firstName, string middleName, string email);
}
