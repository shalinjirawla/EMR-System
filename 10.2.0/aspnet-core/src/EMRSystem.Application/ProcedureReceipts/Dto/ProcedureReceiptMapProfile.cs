using AutoMapper;
using EMRSystem.EmergencyProcedure;
using EMRSystem.ProcedureReceipts;
using EMRSystem.ProcedureReceipts.Dto;
using System.Linq;

namespace EMRSystem.ProcedureReceipts.Dto
{
    public class ProcedureReceiptMapProfile : Profile
    {
        public ProcedureReceiptMapProfile()
        {
            // Entity -> ProcedureReceiptDto
            CreateMap<ProcedureReceipt, ProcedureReceiptDto>()
                .ForMember(dest => dest.PatientName,
                           opt => opt.MapFrom(src => src.Patient != null ? src.Patient.FullName : string.Empty));

            // Create/Update DTO -> Entity
            CreateMap<CreateUpdateProcedureReceiptDto, ProcedureReceipt>();

            // Entity -> View DTO
            CreateMap<ProcedureReceipt, ViewProcedureReceiptDto>()
                .ForMember(dest => dest.PatientName,
                           opt => opt.MapFrom(src => src.Patient != null ? src.Patient.FullName : string.Empty))
                .ForMember(dest => dest.SelectedProcedures,
                           opt => opt.MapFrom(src => src.SelectedEmergencyProcedures));

            // SelectedEmergencyProcedure -> ViewSelectedProcedureDto
            CreateMap <SelectedEmergencyProcedures, ViewSelectedProcedureDto>()
                .ForMember(dest => dest.EmergencyProcedureName,
                           opt => opt.MapFrom(src => src.EmergencyProcedures != null ? src.EmergencyProcedures.Name : string.Empty))
                .ForMember(dest => dest.Price,
                           opt => opt.MapFrom(src => src.EmergencyProcedures != null ? src.EmergencyProcedures.DefaultCharge : 0))
                .ForMember(dest => dest.Status,
                           opt => opt.MapFrom(src => src.Status.ToString()));
        }
    }
}
