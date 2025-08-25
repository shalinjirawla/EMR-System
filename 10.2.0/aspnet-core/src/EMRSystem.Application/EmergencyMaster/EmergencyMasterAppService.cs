using Abp.Application.Services.Dto;
using Abp.Application.Services;
using Abp.Domain.Repositories;
using AutoMapper.Internal.Mappers;
using EMRSystem.DoctorMaster.Dto;
using EMRSystem.DoctorMaster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMRSystem.EmergencyMaster.Dto;

namespace EMRSystem.EmergencyMaster
{
    public class EmergencyMasterAppService : AsyncCrudAppService<
       EMRSystem.Emergency.EmergencyMaster.EmergencyMaster, EmergencyMasterDto, long, PagedResultRequestDto,
       CreateUpdateEmergencyMasterDto, CreateUpdateEmergencyMasterDto>,
       IEmergencyMasterAppService
    {
        public EmergencyMasterAppService(IRepository<EMRSystem.Emergency.EmergencyMaster.EmergencyMaster, long> repository)
            : base(repository)
        {
        }
    }
}
