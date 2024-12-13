using AutoMapper;
using PSPOS.ServiceDefaults.DTOs;
using PSPOS.ServiceDefaults.Models;

namespace PSPOS.ApiService.Data
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();
        }
    }
}