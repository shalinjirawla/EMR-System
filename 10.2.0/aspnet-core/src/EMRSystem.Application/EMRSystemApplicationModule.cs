using Abp.AutoMapper;
using Abp.Dependency;
using Abp.Modules;
using Abp.Reflection.Extensions;
using EMRSystem.Appointments.Dto;
using EMRSystem.Authorization;
using EMRSystem.BillingStaff.Dto;
using EMRSystem.Departments.Dto;
using EMRSystem.Doctor.Dto;
using EMRSystem.EmergencyMaster.Dto;
using EMRSystem.LabReport.Dto;
using EMRSystem.LabTechnician.Dto;
using EMRSystem.Nurse.Dto;
using EMRSystem.Patient_Discharge.Dto;
using EMRSystem.Patients.Dto;
using EMRSystem.Pharmacist.Dto;
using EMRSystem.Prescriptions.Dto;
using EMRSystem.TempStripeData;
using EMRSystem.Visits.Dto;
using EMRSystem.Vitals.Dto;

namespace EMRSystem;

[DependsOn(
    typeof(EMRSystemCoreModule),
    typeof(AbpAutoMapperModule))]
public class EMRSystemApplicationModule : AbpModule
{
    public override void PreInitialize()
    {
        Configuration.Authorization.Providers.Add<EMRSystemAuthorizationProvider>();
        IocManager.Register<ITempStripeDataService, TempStripeDataService>(DependencyLifeStyle.Transient);
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
        Configuration.Modules.AbpAutoMapper().Configurators.Add(cfg => cfg.AddProfile<PrescriptionMapProfile>());
        Configuration.Modules.AbpAutoMapper().Configurators.Add(cfg => cfg.AddProfile<VitalMapProfile>());
        Configuration.Modules.AbpAutoMapper().Configurators.Add(cfg => cfg.AddProfile<PharmacistMapProfile>());
        Configuration.Modules.AbpAutoMapper().Configurators.Add(cfg => cfg.AddProfile<AppointmentMapProfile>());
        Configuration.Modules.AbpAutoMapper().Configurators.Add(cfg => cfg.AddProfile<LabReportMapProfile>());
        Configuration.Modules.AbpAutoMapper().Configurators.Add(cfg => cfg.AddProfile<VisitMapProfile>());
        Configuration.Modules.AbpAutoMapper().Configurators.Add(cfg => cfg.AddProfile<DepartmentMapProfile>());
        Configuration.Modules.AbpAutoMapper().Configurators.Add(cfg => cfg.AddProfile<EmergencyMasterMapProfile>());
        Configuration.Modules.AbpAutoMapper().Configurators.Add(cfg => cfg.AddProfile<PatientDischargeMapProfile>());
    }
}
