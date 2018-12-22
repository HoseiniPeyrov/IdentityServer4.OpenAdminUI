﻿// 
//  AdminClientStore.cs
//  Copyright (c) Johan Boström. All rights reserved.
//  Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
//  

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Stores;
using IdentityServer4.OpenAdminUI.Core.Contracts;
using IdentityServer4.OpenAdminUI.Core.Stores;
using IdentityServer4.OpenAdminUI.EntityFramework.MapperProfiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.OpenAdminUI.EntityFramework.Stores
{
    public class AdminClientStore : IAdminClientStore
    {
        private readonly IConfigurationDbContext context;
        private readonly IMapper mapper;

        public AdminClientStore(IConfigurationDbContext context, ILogger<ClientStore> logger)
        {
            this.context = context;
            mapper = new MapperConfiguration(cfg => { cfg.AddProfile<AdminClientMapperProfile>(); }).CreateMapper();
        }

        public async Task<ClientContract> GetClientAsync(int id)
        {
            var clientEntity = await context.Clients.FirstOrDefaultAsync(c => c.Id == id);
            return mapper.Map<ClientContract>(clientEntity);
        }

        public async Task<List<ClientContract>> GetClientsAsync()
        {
            var clientEntities = await context.Clients
                //.Include(x => x.AllowedGrantTypes)
                //.Include(x => x.RedirectUris)
                //.Include(x => x.PostLogoutRedirectUris)
                //.Include(x => x.AllowedScopes)
                //.Include(x => x.ClientSecrets)
                //.Include(x => x.Claims)
                //.Include(x => x.IdentityProviderRestrictions)
                //.Include(x => x.AllowedCorsOrigins)
                //.Include(x => x.Properties)
                .ToListAsync();

            return clientEntities.Select(mapper.Map<ClientContract>).ToList();
        }

        public async Task<ClientContract> AddClientAsync(ClientContract client)
        {
            var clientEntity = mapper.Map<Client>(client);
            var entityEntry = await context.Clients.AddAsync(clientEntity);
            await context.SaveChangesAsync();

            return mapper.Map<ClientContract>(entityEntry.Entity);
        }

        public async Task<ClientContract> SaveClientAsync(ClientContract client)
        {
            var clientEntity = await context.Clients
                .FirstOrDefaultAsync(c => c.ClientId == client.ClientId);

            mapper.Map(client, clientEntity);

            await context.SaveChangesAsync();

            return mapper.Map<ClientContract>(clientEntity);
        }

        //public async Task<bool> AddClientScopeAsync(string clientId, string scope)
        //{
        //    var client = await GetClientEntityAsync(clientId);
        //    if (client == null)
        //        return false;

        //    if (!client.AllowedScopes.Any(s => s.Scope.Equals(scope, StringComparison.InvariantCultureIgnoreCase)))
        //    {
        //        client.AllowedScopes.Add(new ClientScope
        //        {
        //            Scope = scope
        //        });

        //        await context.SaveChangesAsync();
        //    }

        //    return true;
        //}

        //public async Task<bool> RemoveClientScopeAsync(string clientId, string scope)
        //{
        //    var client = await GetClientEntityAsync(clientId);
        //    if (client == null)
        //        return false;

        //    if (client.AllowedScopes.Any(s => s.Scope.Equals(scope, StringComparison.InvariantCultureIgnoreCase)))
        //    {
        //        client.AllowedScopes.RemoveAll(s => s.Scope.Equals(scope, StringComparison.InvariantCultureIgnoreCase));

        //        await context.SaveChangesAsync();
        //    }

        //    return true;
        //}

        //public async Task<List<ClientSecret>> GetClientSecretsAsync(string clientId)
        //{
        //    var client = await GetClientEntityAsync(clientId);
        //    return client?.ClientSecrets.Select(cs => cs.ToAdminModel()).ToList();
        //}

        //public async Task<ClientSecret> AddClientSecretAsync(string clientId, ClientSecret clientSecret)
        //{
        //    var client = await GetClientEntityAsync(clientId);
        //    if (client == null)
        //        return null;

        //    var entity = clientSecret?.ToEntity();
        //    client.ClientSecrets.Add(entity);

        //    await context.SaveChangesAsync();

        //    return entity?.ToAdminModel();
        //}

        //public async Task<bool> RemoveClientSecretAsync(string clientId, ClientSecret clientSecret)
        //{
        //    var client = await GetClientEntityAsync(clientId);
        //    if (client == null || client.ClientSecrets.All(cs => cs.Id != clientSecret.Id))
        //        return false;

        //    client.ClientSecrets.RemoveAll(cs => cs.Id == clientSecret.Id);
        //    await context.SaveChangesAsync();

        //    return true;
        //}

        #region Private

        private async Task<Client> GetClientEntityAsync(string clientId)
        {
            return await context.Clients
                .Include(x => x.AllowedGrantTypes)
                .Include(x => x.RedirectUris)
                .Include(x => x.PostLogoutRedirectUris)
                .Include(x => x.AllowedScopes)
                .Include(x => x.ClientSecrets)
                .Include(x => x.Claims)
                .Include(x => x.IdentityProviderRestrictions)
                .Include(x => x.AllowedCorsOrigins)
                .Include(x => x.Properties)
                .FirstOrDefaultAsync(c => c.ClientId.Equals(clientId, StringComparison.InvariantCultureIgnoreCase));
        }

        #endregion
    }
}