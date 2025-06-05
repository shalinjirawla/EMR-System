using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace EMRSystem.Controllers
{
    public abstract class EMRSystemControllerBase : AbpController
    {
        protected EMRSystemControllerBase()
        {
            LocalizationSourceName = EMRSystemConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
