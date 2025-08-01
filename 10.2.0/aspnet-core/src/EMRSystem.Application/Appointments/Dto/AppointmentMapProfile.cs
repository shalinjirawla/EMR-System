﻿using AutoMapper;
using EMRSystem.AppointmentReceipt.Dto;
using EMRSystem.Authorization.Roles;
using EMRSystem.Billings;
using EMRSystem.BillingStaff.Dto;
using EMRSystem.Doctor.Dto;
using EMRSystem.Prescriptions;
using EMRSystem.Prescriptions.Dto;
using EMRSystem.Roles.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Appointments.Dto
{
    public class AppointmentMapProfile : Profile
    {
        public AppointmentMapProfile()
        {
            // Main appointment mapping
            CreateMap<Appointment, AppointmentDto>()
                .ForMember(dest => dest.Doctor, opt => opt.MapFrom(src => src.Doctor))
                .ForMember(dest => dest.Patient,opt => opt.MapFrom(src => src.Patient))
                .ReverseMap();

            // Doctor mapping
            CreateMap<EMRSystem.Doctors.Doctor, DoctorDto>();

            // Other mappings
            CreateMap<CreateUpdateAppointmentDto, Appointment>();
            CreateMap<Appointment, CreateUpdateAppointmentDto>()
                //.ForMember(dest => dest.NurseId, opt => opt.MapFrom(src => src.Nurse != null ? src.Nurse.Id : (long?)null))
                .ForMember(dest => dest.DoctorId, opt => opt.MapFrom(src => src.Doctor != null ? src.Doctor.Id : (long?)null))
                .ForMember(dest => dest.PatientId, opt => opt.MapFrom(src => src.Patient != null ? src.Patient.Id : (long?)null));

            CreateMap<EMRSystem.AppointmentReceipt.AppointmentReceipt, AppointmentReceiptDto>()
    .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient.FullName))
    .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor.FullName))
    .ForMember(dest => dest.AppointmentDate,
        opt => opt.MapFrom(src => src.Appointment.AppointmentDate.ToString("dd-MMM-yyyy")));
        }
    }
}
