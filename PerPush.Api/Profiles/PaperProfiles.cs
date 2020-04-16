using AutoMapper;
using PerPush.Api.Entities;
using PerPush.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PerPush.Api.Profiles
{
    public class PaperProfiles:Profile
    {
        public PaperProfiles()
        {
            CreateMap<Paper, PaperDto>()
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author.NickName));
            CreateMap<PaperAddDto, Paper>();
        }
    }
}
