using AutoMapper;
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
            CreateMap<LabTestReceipt, ViewLabTestReceiptDto>()
                // simple props
                .ForMember(dest => dest.TenantId,
                           opt => opt.MapFrom(src => src.TenantId))
                //.ForMember(dest => dest.LabReportTypeId,
                //           opt => opt.MapFrom(src => src.LabReportTypeId))
                .ForMember(dest => dest.PatientId,
                           opt => opt.MapFrom(src => src.PatientId))
                //.ForMember(dest => dest.LabTestFee,
                //           opt => opt.MapFrom(src => src.LabTestFee))
                .ForMember(dest => dest.ReceiptNumber,
                           opt => opt.MapFrom(src => src.ReceiptNumber))
                .ForMember(dest => dest.PaymentDate,
                           opt => opt.MapFrom(src => src.PaymentDate))

                // enum → string
                .ForMember(dest => dest.PaymentMethod,
                           opt => opt.MapFrom(src => src.PaymentMethod.ToString()))
                .ForMember(dest => dest.Status,
                           opt => opt.MapFrom(src => src.Status.ToString()))

                // nested prescription lab test
                //.ForMember(dest => dest.PrescriptionLabTestId,
                //           opt => opt.MapFrom(src => src.PrescriptionLabTestId.Value))

                // deeper navigation: LabReportsType.ReportType → LabReportName
                //.ForMember(dest => dest.LabReportName,
                //           opt => opt.MapFrom(src => src.LabReportType.ReportType))

                // Patient.FullName → PatientName
                .ForMember(dest => dest.PatientName,
                           opt => opt.MapFrom(src => src.Patient.FullName));

                // PrescriptionLabTest.CreatedDate → LabReportDate
                //.ForMember(dest => dest.LabReportDate,
                //           opt => opt.MapFrom(src => src.PrescriptionLabTest.CreatedDate.Value));
        }
    }
}
