// 
//  AdminStoreClientMapperProfile.cs
//  Copyright (c) Johan Boström. All rights reserved.
//  Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
//  

using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.OpenAdminUI.Core.Contracts;

namespace IdentityServer4.OpenAdminUI.EntityFramework.MapperProfiles
{
    public class AdminClientMapperProfile : ClientMapperProfile
    {
        public AdminClientMapperProfile()
        {
            CreateMap<Client, ClientContract>()
                .ReverseMap();
            //CreateMap<ClientSecret, Core.Models.ClientSecret>(MemberList.Destination)
            //    .ForMember(dest => dest.Type, opt => opt.Condition(srs => srs != null))
            //    .ReverseMap();
        }
    }
}