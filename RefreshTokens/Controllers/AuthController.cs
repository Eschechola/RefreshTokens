using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RefreshTokens.Data.Interfaces;
using RefreshTokens.Security;
using RefreshTokens.ViewModels;
using System;
using System.Threading.Tasks;

namespace RefreshTokens.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IClientService _clientService;
        private readonly ISecurityService _securityService;
        private readonly ICacheService _cacheService;

        public AuthController(
            IClientService clientService,
            ISecurityService securityService, 
            ICacheService cacheService)
        {
            _clientService = clientService;
            _securityService = securityService;
            _cacheService = cacheService;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("authenticate")]
        public IActionResult Authenticate([FromBody] AuthenticateViewModel authenticateViewModel)
        {
            try
            {
                var client = _clientService.AuthenticateAsync(authenticateViewModel.Email, authenticateViewModel.Password);

                return Ok(_securityService.CreateToken(client));
            }
            catch (Exception error)
            {
                return StatusCode(400, error.Message);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("refresh-token")]
        public IActionResult RefreshToken([FromBody] RefreshTokenViewModel refreshTokenViewModel)
        {
            try
            {
                if (!_cacheService.ValidateRefreshToken(refreshTokenViewModel.RefreshToken))
                    return BadRequest("Invalid token!");

                var clientCached = _cacheService.GetCachedRefreshToken(refreshTokenViewModel.RefreshToken);
                var client = _clientService.GetClient(clientCached.ClientId);
                
                return Ok(_securityService.CreateToken(client));
            }
            catch (Exception error)
            {
                return StatusCode(400, error.Message);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("my-profile")]
        public IActionResult GetMyProfile()
        {
            try
            {
                var clientId = User.FindFirst("Id")?.Value;
                return Ok(_clientService.GetClient(clientId));
            }
            catch (Exception error)
            {
                return StatusCode(400, error.Message);
            }
        }
    }
}
