using EMRSystem.Patients.Dto;
using EMRSystem.Patients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using EMRSystem.Billings;

namespace EMRSystem.BillingStaff.Dto
{
    class BillingMapProfile : Profile
    {
        public BillingMapProfile()
        {
            CreateMap<Bill, BillingDto>().ReverseMap();
            CreateMap<Bill, CreateUpdateBillingDto>().ReverseMap();
            CreateMap<BillItem, BillItemDto>().ReverseMap();
        }
    }
}
