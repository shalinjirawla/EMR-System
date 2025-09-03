using AutoMapper;
using EMRSystem.MedicineOrder.Dto;
using EMRSystem.Nurse.Dto;
using EMRSystem.Pharmacists;
using EMRSystem.Prescriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Pharmacist.Dto
{
    public class PharmacistMapProfile : Profile
    {
        public PharmacistMapProfile()
        {
            CreateMap<EMRSystem.Pharmacists.Pharmacist, PharmacistDto>().ReverseMap();
            CreateMap<EMRSystem.Pharmacists.Pharmacist, CreateUpdatePharmacistDto>().ReverseMap();

            CreateMap<PharmacistInventory, PharmacistInventoryDto>()
                .ForMember(dest => dest.StockStatus, opt => opt.Ignore())
                .ForMember(dest => dest.ExpiryStatus, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<PharmacistInventory, CreateUpdatePharmacistInventoryDto>().ReverseMap();

            CreateMap<CreateUpdatePharmacistPrescriptionsDto, PharmacistPrescriptions>().ReverseMap();

            CreateMap<PharmacistPrescriptions, PharmacistPrescriptionsDto>()
                  .ForMember(dest => dest.TenantId, opt => opt.MapFrom(src => src.TenantId))
                  .ForMember(dest => dest.PrescriptionId, opt => opt.MapFrom(src => src.Prescriptions.Id))
                  .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => (src.Prescriptions != null && src.Prescriptions.Patient != null) ? src.Prescriptions.Patient.FullName : null))
                  .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => (src.Prescriptions != null && src.Prescriptions.Doctor != null) ? src.Prescriptions.Doctor.FullName : null))
                  .ForMember(dest => dest.IssueDate, opt => opt.MapFrom(src => src.IssueDate))
                  .ForMember(dest => dest.Order_Status, opt => opt.MapFrom(src => src.Order_Status))
                  .ForMember(dest => dest.PharmacyNotes, opt => opt.MapFrom(src => src.PharmacyNotes))
                  .ForMember(dest => dest.CollectionStatus, opt => opt.MapFrom(src => src.CollectionStatus))
                  .ForMember(dest => dest.PickedUpBy, opt => opt.MapFrom(src => src.PickedUpBy))
                  .ReverseMap();
        }
    }
}
