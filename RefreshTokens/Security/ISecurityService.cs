using RefreshTokens.Data.Entities;
using System.Threading.Tasks;

namespace RefreshTokens.Security
{
    public interface ISecurityService
    {
        Token CreateToken(Client client);
    }
}
