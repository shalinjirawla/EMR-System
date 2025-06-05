using Abp.Authorization;
using EMRSystem.Authorization.Roles;
using EMRSystem.Authorization.Users;

namespace EMRSystem.Authorization;

public class PermissionChecker : PermissionChecker<Role, User>
{
    public PermissionChecker(UserManager userManager)
        : base(userManager)
    {
    }
}
