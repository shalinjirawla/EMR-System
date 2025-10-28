using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.DeathRecord.Dto
{
    public class DeathRecordMapProfile : Profile
    {
        public DeathRecordMapProfile()
        {
            CreateMap<DeathRecord, DeathRecordDto>()
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient != null ? src.Patient.FullName : string.Empty))
                .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor != null ? src.Doctor.FullName : string.Empty))
                .ForMember(dest => dest.NurseName, opt => opt.MapFrom(src => src.Nurse != null ? src.Nurse.FullName : string.Empty));

            CreateMap<CreateUpdateDeathRecordDto, DeathRecord>();
        }
    }
}
