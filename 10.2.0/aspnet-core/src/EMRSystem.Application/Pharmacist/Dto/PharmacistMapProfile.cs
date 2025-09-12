using AutoMapper;
using EMRSystem.MedicineOrder.Dto;
using EMRSystem.Nurse.Dto;
using EMRSystem.Pharmacists;
using EMRSystem.Prescriptions;
using EMRSystem.Prescriptions.Dto;
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
            CreateMap<PharmacistPrescriptionItemWithUnitPriceDto, PrescriptionItem>().ReverseMap();

            CreateMap<PharmacistPrescriptions, PharmacistPrescriptionsDto>()
                  .ForMember(dest => dest.TenantId, opt => opt.MapFrom(src => src.TenantId))
                  .ForMember(dest => dest.PrescriptionId, opt => opt.MapFrom(src => src.Prescriptions.Id))
                  .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => (src.Prescriptions != null && src.Prescriptions.Patient != null) ? src.Prescriptions.Patient.FullName : null))
                  .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => (src.Prescriptions != null && src.Prescriptions.Doctor != null) ? src.Prescriptions.Doctor.FullName : null))
                  .ForMember(dest => dest.IssueDate, opt => opt.MapFrom(src => src.IssueDate))
                  .ForMember(dest => dest.IsPaid, opt => opt.MapFrom(src => src.IsPaid))
                  .ForMember(dest => dest.PharmacyNotes, opt => opt.MapFrom(src => src.PharmacyNotes))
                  .ForMember(dest => dest.CollectionStatus, opt => opt.MapFrom(src => src.CollectionStatus))
                  .ForMember(dest => dest.PickedUpByNurse, opt => opt.MapFrom(src => src.PickedUpByNurse))
                  .ForMember(dest => dest.PickedUpByPatient, opt => opt.MapFrom(src => src.PickedUpByPatient))
                  .ForMember(dest => dest.GrandTotal, opt => opt.MapFrom(src => src.GrandTotal))
                  .ForMember(dest => dest.prescriptionItems, opt => opt.MapFrom(src => (src.Prescriptions != null && src.Prescriptions.Items != null) ? src.Prescriptions.Items : null))
                  .ReverseMap();
            // PharmacistMapProfile class me yeh mapping add karo
            CreateMap<PharmacistPrescriptions, ViewPharmacistPrescriptionsDto>()
                .ForMember(dest => dest.TenantName, opt => opt.Ignore()) // Ye hum manually set karenge
                .ForMember(dest => dest.PharmacistPrescriptionId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.PrescriptionId, opt => opt.MapFrom(src => src.PrescriptionId.Value))
                .ForMember(dest => dest.PatientId, opt => opt.MapFrom(src => src.Prescriptions.Patient.Id))
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Prescriptions.Patient.FullName))
                .ForMember(dest => dest.PatientDateOfBirth, opt => opt.MapFrom(src => src.Prescriptions.Patient.DateOfBirth))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Prescriptions.Patient.Gender))
                .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Prescriptions.Doctor.FullName))
                .ForMember(dest => dest.DoctorRegistrationNumber, opt => opt.MapFrom(src => src.Prescriptions.Doctor.RegistrationNumber))
                .ForMember(dest => dest.IssueDate, opt => opt.MapFrom(src => src.IssueDate.Value))
                .ForMember(dest => dest.CollectionStatus, opt => opt.MapFrom(src => src.CollectionStatus))
                .ForMember(dest => dest.IsPaid, opt => opt.MapFrom(src => src.IsPaid))
                .ForMember(dest => dest.PharmacyNotes, opt => opt.MapFrom(src => src.PharmacyNotes))
                .ForMember(dest => dest.ReceiptNumber, opt => opt.MapFrom(src => src.ReceiptNumber))
                .ForMember(dest => dest.PaymentIntentId, opt => opt.MapFrom(src => src.PaymentIntentId))
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod != null ? src.PaymentMethod : null))
                .ForMember(dest => dest.PickedUpByNurseId, opt => opt.MapFrom(src => src.PickedUpByNurse))
                .ForMember(dest => dest.PickedUpByPatientId, opt => opt.MapFrom(src => src.PickedUpByPatient))
                .ForMember(dest => dest.PickedUpByNurse, opt => opt.MapFrom(src => src.Nurse.FullName))
                .ForMember(dest => dest.PickedUpByPatient, opt => opt.MapFrom(src => src.Patient.FullName))
                .ForMember(dest => dest.GrandTotal, opt => opt.MapFrom(src => src.GrandTotal))
                .ForMember(dest => dest.PrescriptionItems, opt => opt.MapFrom(src => src.PrescriptionItems));
        }
    }
}
