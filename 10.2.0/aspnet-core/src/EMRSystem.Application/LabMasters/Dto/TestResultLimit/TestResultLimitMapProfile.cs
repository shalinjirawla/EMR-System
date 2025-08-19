using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabMasters.Dto.TestResultLimit
{
    public class TestResultLimitMapProfile : Profile
    {
        public TestResultLimitMapProfile()
        {
            CreateMap<EMRSystem.LabMasters.TestResultLimit, TestResultLimitDto>()
                .ForMember(dest => dest.LabTestName, opt => opt.MapFrom(src => src.LabTest.Name)); // <-- Required

            CreateMap<CreateUpdateTestResultLimitDto, EMRSystem.LabMasters.TestResultLimit>();
        }
    }
}
