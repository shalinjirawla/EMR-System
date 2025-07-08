using AutoMapper;
using EMRSystem.Authorization.Users;
using EMRSystem.Users.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Departments.Dto
{
    public class DepartmentMapProfile : Profile
    {
        public DepartmentMapProfile()
        {
            CreateMap<DepartmentListDto, Department>().ReverseMap();
            CreateMap<CreateUpdateDepartmentDto, Department>().ReverseMap();
        }
    }
}
