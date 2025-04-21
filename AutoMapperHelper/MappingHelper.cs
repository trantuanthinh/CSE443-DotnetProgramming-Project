using AutoMapper;
using Project.DTO;
using Project.Models;

namespace Project.AutoMapperHelper
{
    public class MappingHelper : Profile
    {
        public MappingHelper()
        {
            // ----- CreateMap<A,B> => Map from A to B

            CreateMap<GoogleRequest, User>();
        }
    }
}
