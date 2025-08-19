using AutoMapper;
using EMRSystem.Patients;
using EMRSystem.Patients.Dto;
using EMRSystem.PrescriptionLabTest.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabTestReceipt.Dto
{
    public class LabTestReceiptMapProfile : Profile
    {
        public LabTestReceiptMapProfile()
        {
            // Main mapping
            CreateMap<LabTestReceipt, LabTestReceiptDto>()
                .ForMember(dest => dest.PrescriptionLabTests,
                    opt => opt.MapFrom(src => src.PrescriptionLabTests));

            // Patient mapping
            CreateMap<Patient, PatientDto>();

            // PrescriptionLabTest mapping
            CreateMap<EMRSystem.LabReports.PrescriptionLabTest, PrescriptionLabTestDto>()
                .ForMember(dest => dest.ReportTypeName,
                    opt => opt.MapFrom(src => src.LabReportsType.ReportType))
                .ForMember(dest => dest.PackageName,
                    opt => opt.MapFrom(src => src.HealthPackage.PackageName));

            // View mapping
            CreateMap<LabTestReceipt, ViewLabTestReceiptDto>()
                .ForMember(dest => dest.PatientName,
                    opt => opt.MapFrom(src => src.Patient.FullName))
                .ForMember(dest => dest.PaymentMethodDisplay,
                    opt => opt.MapFrom(src => src.PaymentMethod.ToString()))
                .ForMember(dest => dest.StatusDisplay,
                    opt => opt.MapFrom(src => src.Status.ToString()));
        }
    }
}
