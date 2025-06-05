using EMRSystem.Configuration.Dto;
using System.Threading.Tasks;

namespace EMRSystem.Configuration;

public interface IConfigurationAppService
{
    Task ChangeUiTheme(ChangeUiThemeInput input);
}
