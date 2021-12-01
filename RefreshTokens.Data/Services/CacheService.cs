using Microsoft.Extensions.Caching.Distributed;
using RefreshTokens.Data.Interfaces;
using System;
using System.Text.Json;

namespace RefreshTokens.Data.Services
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _distributedCache;

        public CacheService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public RefreshTokenDTO GetCachedRefreshToken(string refreshToken)
        {
            var refreshTokenCached = _distributedCache.GetString(refreshToken);

            return JsonSerializer.Deserialize<RefreshTokenDTO>(refreshTokenCached);
        }

        public bool SaveRefreshToken(string clientId, string refreshToken, DateTime expiresAt)
        {
            var options = new DistributedCacheEntryOptions();
            options.SetAbsoluteExpiration(expiresAt);

            var refreshTokenData = new RefreshTokenDTO
            {
                ClientId = clientId,
                RefreshToken = refreshToken
            };

            _distributedCache.SetString(refreshToken, JsonSerializer.Serialize(refreshTokenData), options);

            return true;
        }

        public bool ValidateRefreshToken(string refreshToken)
        {
            var refreshTokenCached = _distributedCache.GetString(refreshToken);

            if (refreshTokenCached == null)
                throw new Exception("Refresh token doesn't exists, please login again");

            return true;
        }
    }
}
