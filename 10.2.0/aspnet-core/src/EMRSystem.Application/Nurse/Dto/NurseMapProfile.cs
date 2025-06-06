using AutoMapper;

namespace EMRSystem.Nurse.Dto
{
    public class NurseMapProfile:Profile
    {
        public NurseMapProfile()
        {
            CreateMap<EMRSystem.Nurses.Nurse, NurseDto>().ReverseMap();
            CreateMap<EMRSystem.Nurses.Nurse, CreateUpdateNurseDto>().ReverseMap();
        }
    }
}
