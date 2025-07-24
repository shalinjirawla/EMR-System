using AutoMapper;
using EMRSystem.Doctor.Dto;
using EMRSystem.Invoices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.AppointmentReceipt.Dto
{
    public class AppointmentReceiptMapProfile : Profile
    {
        public AppointmentReceiptMapProfile()
        {
            // Main receipt mapping
            CreateMap<AppointmentReceipt, AppointmentReceiptDto>()
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient.FullName))
                .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor.FullName))
                .ForMember(dest => dest.AppointmentDate,
                    opt => opt.MapFrom(src => src.Appointment.AppointmentDate.ToString("dd-MMM-yyyy")))
                .ReverseMap();

            // Create receipt mapping
            CreateMap<CreateAppointmentReceiptDto, AppointmentReceipt>()
                .ForMember(dest => dest.PaymentMethod,
                    opt => opt.MapFrom(src => (PaymentMethod)Enum.Parse(typeof(PaymentMethod), src.PaymentMethod)));

            // Doctor mapping (reuse existing)
            CreateMap<EMRSystem.Doctors.Doctor, DoctorDto>();
        }
    }
}
