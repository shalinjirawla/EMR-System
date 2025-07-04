using AutoMapper;
using EMRSystem.Authorization.Users;
using EMRSystem.Users.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Visits.Dto
{
    class VisitMapProfile : Profile
    {
        public VisitMapProfile()
        {
            CreateMap<VisitListDto, Visit>().ReverseMap();
            CreateMap<CreateUpdateVisitDto, Visit>().ReverseMap();
        }
    }
}
