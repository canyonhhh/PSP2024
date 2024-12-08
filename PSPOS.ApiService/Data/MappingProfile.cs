using AutoMapper;
using PSPOS.ServiceDefaults.DTOs;
using PSPOS.ServiceDefaults.Models;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<UserDto, User>()
            .ForMember(dest => dest.PinHash, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());
    }
}