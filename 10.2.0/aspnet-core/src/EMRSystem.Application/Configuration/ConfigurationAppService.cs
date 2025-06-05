using Abp.Authorization;
using Abp.Runtime.Session;
using EMRSystem.Configuration.Dto;
using System.Threading.Tasks;

namespace EMRSystem.Configuration;

[AbpAuthorize]
public class ConfigurationAppService : EMRSystemAppServiceBase, IConfigurationAppService
{
    public async Task ChangeUiTheme(ChangeUiThemeInput input)
    {
        await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
    }
}
