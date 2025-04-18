﻿using AutoMapper;
using BankSystem.Server.Dtos;
using BankSystem.Server.Services.Dtos;

namespace BankSystem.Server.MappingProfile
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // CreateMap<Source, Destination>();
            CreateMap<LoginDto, LoginServiceDto>();
        }
    }
}
