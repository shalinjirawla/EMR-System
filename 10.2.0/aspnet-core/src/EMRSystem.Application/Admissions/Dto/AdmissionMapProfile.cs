using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Admissions.Dto
{
    public class AdmissionMapProfile : Profile
    {
        public AdmissionMapProfile()
        {
            CreateMap<EMRSystem.Admission.Admission, AdmissionDto>()
                    .ForMember(dest => dest.PatientName,
                        opt => opt.MapFrom(src => src.Patient != null ? src.Patient.FullName : null))
                    .ForMember(dest => dest.DoctorName,
                        opt => opt.MapFrom(src => src.Doctor != null ? src.Doctor.FullName : null))
                    .ForMember(dest => dest.NurseName,
                        opt => opt.MapFrom(src => src.Nurse != null ? src.Nurse.FullName : null))
                    .ForMember(dest => dest.RoomNumber,
                        opt => opt.MapFrom(src => src.Room != null ? src.Room.RoomNumber : null))
                    .ForMember(dest => dest.BedNumber,
                        opt => opt.MapFrom(src => src.Bed != null ? src.Bed.BedNumber : null))
                    .ForMember(dest => dest.RoomTypeName,
                        opt => opt.MapFrom(src => src.Room != null && src.Room.RoomTypeMaster != null ? src.Room.RoomTypeMaster.TypeName : null))
                    .ForMember(dest => dest.RoomTypePricePerDay,
                        opt => opt.MapFrom(src => src.Room != null && src.Room.RoomTypeMaster != null ? src.Room.RoomTypeMaster.DefaultPricePerDay : (decimal?)null));

            CreateMap<CreateUpdateAdmissionDto, EMRSystem.Admission.Admission>();

        }
    }
}
