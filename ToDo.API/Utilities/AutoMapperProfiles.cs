//Class used for configuration of AutoMapper profiles

using AutoMapper;
using ToDo.Models.DTOs;
using ToDo.Models.Models;
namespace ToDo.Utilities
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<TDTaskDTO, TDTask>();
        }
    }
}
