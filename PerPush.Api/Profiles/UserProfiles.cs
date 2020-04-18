using AutoMapper;
using PerPush.Api.Entities;
using PerPush.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace PerPush.Api.Profiles
{
    public class UserProfiles:Profile
    {
        public UserProfiles()
        {
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.LastName + src.FirstName));
            CreateMap<User, UserInfoDto>();
            
            CreateMap<UserRegisteredDto, User>();
            CreateMap<User, UserUpdateDto>();
            CreateMap<UserUpdateDto, User>();
        }
    }
}
