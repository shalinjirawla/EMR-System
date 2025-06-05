using Abp.Modules;
using Abp.Reflection.Extensions;
using EMRSystem.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace EMRSystem.Web.Host.Startup
{
    [DependsOn(
       typeof(EMRSystemWebCoreModule))]
    public class EMRSystemWebHostModule : AbpModule
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public EMRSystemWebHostModule(IWebHostEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(EMRSystemWebHostModule).GetAssembly());
        }
    }
}
