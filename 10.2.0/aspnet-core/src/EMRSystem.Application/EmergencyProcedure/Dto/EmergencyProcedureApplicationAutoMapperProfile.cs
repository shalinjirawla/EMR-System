using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.EmergencyProcedure.Dto
{
    public class EmergencyProcedureApplicationAutoMapperProfile : Profile
    {
        public EmergencyProcedureApplicationAutoMapperProfile()
        {
            // EmergencyProcedure master mapping
            CreateMap<EmergencyProcedure, EmergencyProcedureDto>();
            CreateMap<CreateUpdateEmergencyProcedureDto, EmergencyProcedure>();

            // SelectedEmergencyProcedures mapping
            CreateMap<SelectedEmergencyProcedures, SelectedEmergencyProceduresDto>()
                .ForMember(dest => dest.ProcedureName, opt => opt.MapFrom(src => src.EmergencyProcedures.Name))
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Prescriptions.Patient.FullName));




            CreateMap<CreateUpdateSelectedEmergencyProceduresDto, SelectedEmergencyProcedures>();

            // ReverseMap if needed for DTO to Entity
            CreateMap<SelectedEmergencyProceduresDto, SelectedEmergencyProcedures>()
                .ForMember(dest => dest.EmergencyProcedures, opt => opt.Ignore())
                .ForMember(dest => dest.Prescriptions, opt => opt.Ignore());
        }
    }
}
