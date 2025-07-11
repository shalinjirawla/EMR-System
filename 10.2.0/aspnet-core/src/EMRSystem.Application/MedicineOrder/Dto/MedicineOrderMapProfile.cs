using AutoMapper;
using EMRSystem.Nurse.Dto;
using EMRSystem.Patients;
using EMRSystem.Patients.Dto;
using EMRSystem.Prescriptions;
using EMRSystem.Prescriptions.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.MedicineOrder.Dto
{
    public class MedicineOrderMapProfile : Profile
    {
        public MedicineOrderMapProfile()
        {
            // Main Order
            CreateMap<MedicineOrder, MedicineOrderDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority.ToString()));
            CreateMap<MedicineOrderDto, MedicineOrder>();

            CreateMap<MedicineOrder, CreateUpdateMedicineOrderDto>()
                .ForMember(dest => dest.NurseId, opt => opt.MapFrom(src => src.NurseId))
                .ForMember(dest => dest.PatientId, opt => opt.MapFrom(src => src.PatientId))
                .ForMember(dest => dest.Nurse, opt => opt.MapFrom(src => src.Nurse))
                .ForMember(dest => dest.Patient, opt => opt.MapFrom(src => src.Patient))
                .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority.ToString()))
                .ReverseMap();

            // Items
            CreateMap<MedicineOrderItem, CreateUpdateMedicineOrderItemDto>().ReverseMap();
            CreateMap<MedicineOrderItem, MedicineOrderItemDto>()
    .ForMember(dest => dest.MedicineName, opt => opt.MapFrom(src => src.Medicine.MedicineName))
    .ReverseMap();


            // Nurse and Patient
            CreateMap<EMRSystem.Nurses.Nurse, NurseDto>();
            CreateMap<Patient, PatientDto>();
        }
    }

}
