﻿using Abp.AspNetCore;
using Abp.AspNetCore.TestBase;
using Abp.Modules;
using Abp.Reflection.Extensions;
using EMRSystem.EntityFrameworkCore;
using EMRSystem.Web.Startup;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace EMRSystem.Web.Tests;

[DependsOn(
    typeof(EMRSystemWebMvcModule),
    typeof(AbpAspNetCoreTestBaseModule)
)]
public class EMRSystemWebTestModule : AbpModule
{
    public EMRSystemWebTestModule(EMRSystemEntityFrameworkModule abpProjectNameEntityFrameworkModule)
    {
        abpProjectNameEntityFrameworkModule.SkipDbContextRegistration = true;
    }

    public override void PreInitialize()
    {
        Configuration.UnitOfWork.IsTransactional = false; //EF Core InMemory DB does not support transactions.
    }

    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(typeof(EMRSystemWebTestModule).GetAssembly());
    }

    public override void PostInitialize()
    {
        IocManager.Resolve<ApplicationPartManager>()
            .AddApplicationPartsIfNotAddedBefore(typeof(EMRSystemWebMvcModule).Assembly);
    }
}