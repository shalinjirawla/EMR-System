using Abp.Events.Bus;
using Abp.Modules;
using Abp.Reflection.Extensions;
using EMRSystem.Configuration;
using EMRSystem.EntityFrameworkCore;
using EMRSystem.Migrator.DependencyInjection;
using Castle.MicroKernel.Registration;
using Microsoft.Extensions.Configuration;

namespace EMRSystem.Migrator;

[DependsOn(typeof(EMRSystemEntityFrameworkModule))]
public class EMRSystemMigratorModule : AbpModule
{
    private readonly IConfigurationRoot _appConfiguration;

    public EMRSystemMigratorModule(EMRSystemEntityFrameworkModule abpProjectNameEntityFrameworkModule)
    {
        abpProjectNameEntityFrameworkModule.SkipDbSeed = true;

        _appConfiguration = AppConfigurations.Get(
            typeof(EMRSystemMigratorModule).GetAssembly().GetDirectoryPathOrNull()
        );
    }

    public override void PreInitialize()
    {
        Configuration.DefaultNameOrConnectionString = _appConfiguration.GetConnectionString(
            EMRSystemConsts.ConnectionStringName
        );

        Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
        Configuration.ReplaceService(
            typeof(IEventBus),
            () => IocManager.IocContainer.Register(
                Component.For<IEventBus>().Instance(NullEventBus.Instance)
            )
        );
    }

    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(typeof(EMRSystemMigratorModule).GetAssembly());
        ServiceCollectionRegistrar.Register(IocManager);
    }
}
