using RefreshTokens.Data.Entities;

namespace RefreshTokens.Data.Interfaces
{
    public interface IClientService
    {
        Client GetClient(string clientId);
        Client AuthenticateAsync(string email, string password);
    }
}
