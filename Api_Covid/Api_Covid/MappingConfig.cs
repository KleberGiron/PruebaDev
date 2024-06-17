using Api_Covid.Models;
using Api_Covid.Models.Dto;
using AutoMapper;

namespace Api_Covid
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<Empleado, EmpleadosDto>();
            CreateMap<EmpleadosDto, Empleado>();

            CreateMap<Empleado, EmpleadosCreateDto>();
            CreateMap<EmpleadosCreateDto, Empleado>();

            CreateMap<Empleado, EmpleadosUpdateDto>();
            CreateMap<EmpleadosUpdateDto, Empleado>();
        }
    }
}
