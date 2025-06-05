using Abp.Zero.EntityFrameworkCore;
using EMRSystem.Authorization.Roles;
using EMRSystem.Authorization.Users;
using EMRSystem.MultiTenancy;
using Microsoft.EntityFrameworkCore;

namespace EMRSystem.EntityFrameworkCore;

public class EMRSystemDbContext : AbpZeroDbContext<Tenant, Role, User, EMRSystemDbContext>
{
    /* Define a DbSet for each entity of the application */

    public EMRSystemDbContext(DbContextOptions<EMRSystemDbContext> options)
        : base(options)
    {
    }
}
