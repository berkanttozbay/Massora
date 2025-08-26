using AutoMapper;
using Massora.Domain.Entities;
using Massora.Business.DTOs;

namespace Massora.Business.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            // Company mappings
            CreateMap<Company, CompanyDto>();
            CreateMap<CreateCompanyDto, Company>();
            CreateMap<UpdateCompanyDto, Company>();

            // PartnerCompany mappings
            CreateMap<PartnerCompany, PartnerCompanyDto>().ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company.Name)); 
            CreateMap<CreatePartnerCompanyDto, PartnerCompany>();
            CreateMap<UpdatePartnerCompanyDto, PartnerCompany>();

            // Vehicle mappings
            CreateMap<Vehicle, VehicleDto>().ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company.Name));
            CreateMap<CreateVehicleDto, Vehicle>();
            CreateMap<Vehicle, CreateVehicleDto>();
            CreateMap<UpdateVehicleDto, Vehicle>();

            // Driver mappings
            CreateMap<Driver, DriverDto>().ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company.Name));
            CreateMap<CreateDriverDto, Driver>();
            CreateMap<UpdateDriverDto, Driver>();

            // WorkHistory mappings
            CreateMap<WorkHistory, WorkHistoryDto>().ForMember(dest => dest.DriverName, opt => opt.MapFrom(src => src.Driver.Name))
                                                    .ForMember(dest => dest.VehicleType, opt => opt.MapFrom(src => src.Vehicle.VehicleType))
                                                    .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company.Name));
            CreateMap<CreateWorkHistoryDto, WorkHistory>();
            CreateMap<UpdateWorkHistoryDto, WorkHistory>();

            // VehicleFuelHistory mappings
            CreateMap<VehicleFuelHistory, VehicleFuelHistoryDto>().ForMember(dest => dest.VehicleType, opt => opt.MapFrom(src => src.Vehicle.VehicleType))
                                                                   .ForMember(dest => dest.DriverName, opt => opt.MapFrom(src => src.Driver.Name));
            CreateMap<CreateVehicleFuelHistoryDto, VehicleFuelHistory>();
            CreateMap<VehicleFuelHistory, CreateVehicleFuelHistoryDto>();
            CreateMap<UpdateVehicleFuelHistoryDto, VehicleFuelHistory>();
        }
    }
} 