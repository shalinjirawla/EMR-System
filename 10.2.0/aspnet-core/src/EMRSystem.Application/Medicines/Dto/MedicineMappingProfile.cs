using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Medicines.Dto
{
    public class MedicineMappingProfile : Profile
    {
        public MedicineMappingProfile()
        {
            // MedicineMaster
            CreateMap<MedicineMaster, MedicineMasterDto>()
                .ForMember(dest => dest.FormName, opt => opt.MapFrom(src => src.Form.Name))
                .ForMember(dest => dest.StrengthUnitName, opt => opt.MapFrom(src => src.StrengthUnit.Name));

            CreateMap<CreateUpdateMedicineMasterDto, MedicineMaster>();

            // MedicineStock
            CreateMap<MedicineStock, MedicineStockDto>()
                .ForMember(dest => dest.MedicineName, opt => opt.MapFrom(src => src.MedicineMaster.Name));

            CreateMap<CreateUpdateMedicineStockDto, MedicineStock>();
        }
    }
}
