using AutoMapper;
using EMRSystem.LabReports;
using EMRSystem.LabTechnician.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Prescriptions.Dto
{
    class PrescriptionMapProfile : Profile
    {
        public PrescriptionMapProfile()
        {
            CreateMap<Prescription, PrescriptionDto>().ReverseMap();
            CreateMap<Prescription, CreateUpdatePrescriptionDto>().ReverseMap();
            CreateMap<PrescriptionItem, PrescriptionItemDto>().ReverseMap();
            CreateMap<PrescriptionItem, CreateUpdatePrescriptionItemDto>().ReverseMap();
        }
    }
}
