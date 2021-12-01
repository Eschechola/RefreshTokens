using System;

namespace RefreshTokens.Data.Interfaces
{
    public interface ICacheService
    {
        bool SaveRefreshToken(string clientId, string refreshToken, DateTime expiresAt);
        bool ValidateRefreshToken(string refreshToken);
        RefreshTokenDTO GetCachedRefreshToken(string refreshToken);
    }
}
