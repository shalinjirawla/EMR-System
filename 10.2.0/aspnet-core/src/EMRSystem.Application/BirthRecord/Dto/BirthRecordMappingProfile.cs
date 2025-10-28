using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.BirthRecord.Dto
{
    public class BirthRecordMappingProfile : Profile
    {
        public BirthRecordMappingProfile()
        {
            CreateMap<BirthRecord, BirthRecordDto>()
             .ForMember(dest => dest.MotherName, opt => opt.MapFrom(src => src.Mother != null ? src.Mother.FullName : null))
             .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor != null ? src.Doctor.FullName : null))
             .ForMember(dest => dest.NurseName, opt => opt.MapFrom(src => src.Nurse != null ? src.Nurse.FullName : null));

            CreateMap<CreateUpdateBirthRecordDto, BirthRecord>();
        }
    }
}
