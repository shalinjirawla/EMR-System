using Abp.Application.Services;
using Abp.Application.Services.Dto;
using EMRSystem.MedicineForms.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.MedicineForms
{
    public interface IMedicineFormMasterAppService :
        IAsyncCrudAppService<MedicineFormMasterDto, long, PagedAndSortedResultRequestDto, CreateUpdateMedicineFormMasterDto, CreateUpdateMedicineFormMasterDto>
    {
    }
}
