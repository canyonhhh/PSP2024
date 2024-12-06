using AutoMapper;
using PSPOS.ServiceDefaults.Models;
using PSPOS.ServiceDefaults.DTOs;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<UserDTO, User>()
            .ForMember(dest => dest.PinHash, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());
    }
}
