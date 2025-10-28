using Abp.Application.Services;
using Abp.Application.Services.Dto;
using EMRSystem.BirthRecord.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.BirthRecord
{
    public interface IBirthRecordAppService :
       IAsyncCrudAppService<BirthRecordDto, long, PagedBirthRecordResultRequestDto, CreateUpdateBirthRecordDto>
    {
    }
}
