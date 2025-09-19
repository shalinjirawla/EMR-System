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
            CreateMap<MedicineMaster, MedicineMasterDto>();
            CreateMap<MedicineMasterDto, MedicineMaster>();
            CreateMap<CreateUpdateMedicineMasterDto, MedicineMaster>();

            CreateMap<PurchaseInvoice, PurchaseInvoiceDto>();
            CreateMap<PurchaseInvoiceDto, PurchaseInvoice>();
            CreateMap<CreateUpdatePurchaseInvoiceDto, PurchaseInvoice>();

            CreateMap<PurchaseInvoiceItem, PurchaseInvoiceItemDto>()
                .ForMember(dest => dest.MedicineName, opt => opt.MapFrom(src => src.MedicineMaster.Name));

            CreateMap<PurchaseInvoiceItemDto, PurchaseInvoiceItem>();
            CreateMap<CreateUpdatePurchaseInvoiceItemDto, PurchaseInvoiceItem>();

            CreateMap<MedicineStock, MedicineStockDto>();
            CreateMap<MedicineStockDto, MedicineStock>();
            CreateMap<CreateUpdateMedicineStockDto, MedicineStock>();
            CreateMap<MedicineMaster, MedicineLookupDto>()
                .ForMember(d => d.DosageOption,
                    opt => opt.MapFrom(s => s.Strength + " " + s.StrengthUnit.Name));
        }
    }
}
