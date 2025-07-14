using Application.DTOs;
using AutoMapper;
using Domain;

namespace Application.Core
{
    public class MappingProfiles:Profile
    {
        public MappingProfiles()
        {
            //This is used in EditActivity.cs
            CreateMap<Activity, Activity>();

            CreateMap<CreateActivityDto, Activity>();
            CreateMap<EditActivityDto, Activity>();
        }
    }
}