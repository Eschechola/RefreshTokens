using RefreshTokens.Data.Entities;
using RefreshTokens.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RefreshTokens.Data.Services
{
    public class ClientService : IClientService
    {
        private static IList<Client> _clients = new List<Client>
        {
            new Client { Id = Guid.NewGuid().ToString(), Name = "Lucas Gabriel", Email = "lucas@eu.com", Password = "123321" },
            new Client { Id = Guid.NewGuid().ToString(), Name = "Pedro José", Email = "pedro@eu.com", Password = "123321" },
            new Client { Id = Guid.NewGuid().ToString(), Name = "Gabriel Silva", Email = "gabriel@eu.com", Password = "123321" }
        };

        public Client AuthenticateAsync(string email, string password)
        {
            var client = _clients.Where(x =>
                x.Email.ToLower() == email.ToLower() &&
                x.Password.ToLower() == password.ToLower())
            .FirstOrDefault();

            if (client == null)
                throw new Exception("Email and/or password mismatch!");

            return client;
        }

        public Client GetClient(string clientId)
        {
            var client = _clients.Where(x => x.Id == clientId)
                .FirstOrDefault();

            return client;
        }
    }
}
