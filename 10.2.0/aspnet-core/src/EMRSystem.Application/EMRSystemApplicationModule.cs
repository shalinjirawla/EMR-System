﻿using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using EMRSystem.Authorization;
using EMRSystem.BillingStaff.Dto;
using EMRSystem.Doctor.Dto;
using EMRSystem.LabTechnician.Dto;
using EMRSystem.Nurse.Dto;
using EMRSystem.Patients.Dto;

namespace EMRSystem;

[DependsOn(
    typeof(EMRSystemCoreModule),
    typeof(AbpAutoMapperModule))]
public class EMRSystemApplicationModule : AbpModule
{
    public override void PreInitialize()
    {
        Configuration.Authorization.Providers.Add<EMRSystemAuthorizationProvider>();
    }

    public override void Initialize()
    {
        var thisAssembly = typeof(EMRSystemApplicationModule).GetAssembly();

        IocManager.RegisterAssemblyByConvention(thisAssembly);

        Configuration.Modules.AbpAutoMapper().Configurators.Add(
            // Scan the assembly for classes which inherit from AutoMapper.Profile
            cfg => cfg.AddMaps(thisAssembly)
        );

        Configuration.Modules.AbpAutoMapper().Configurators.Add(cfg => cfg.AddProfile<BillingMapProfile>());
        Configuration.Modules.AbpAutoMapper().Configurators.Add(cfg => cfg.AddProfile<DoctorMapProfile>());
        Configuration.Modules.AbpAutoMapper().Configurators.Add(cfg => cfg.AddProfile<LabMapProfile>());
        Configuration.Modules.AbpAutoMapper().Configurators.Add(cfg => cfg.AddProfile<NurseMapProfile>());
        Configuration.Modules.AbpAutoMapper().Configurators.Add(cfg => cfg.AddProfile<PatientMapProfile>());
    }
}
