using AutoMapper;
using EMRSystem.StrengthUnitMaster.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.MedicineForms.Dto
{
    
    public class MedicineFormsMapProfile : Profile
    {
        public MedicineFormsMapProfile()
        {
            CreateMap<EMRSystem.MedicineFormMaster.MedicineFormMaster, MedicineFormMasterDto>();
            CreateMap<CreateUpdateMedicineFormMasterDto, EMRSystem.MedicineFormMaster.MedicineFormMaster>();
        }
    }
}
