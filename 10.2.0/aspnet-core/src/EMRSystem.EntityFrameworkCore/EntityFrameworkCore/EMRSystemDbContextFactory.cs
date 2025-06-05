using EMRSystem.Configuration;
using EMRSystem.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace EMRSystem.EntityFrameworkCore;

/* This class is needed to run "dotnet ef ..." commands from command line on development. Not used anywhere else */
public class EMRSystemDbContextFactory : IDesignTimeDbContextFactory<EMRSystemDbContext>
{
    public EMRSystemDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<EMRSystemDbContext>();

        /*
         You can provide an environmentName parameter to the AppConfigurations.Get method. 
         In this case, AppConfigurations will try to read appsettings.{environmentName}.json.
         Use Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") method or from string[] args to get environment if necessary.
         https://docs.microsoft.com/en-us/ef/core/cli/dbcontext-creation?tabs=dotnet-core-cli#args
         */
        var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());

        EMRSystemDbContextConfigurer.Configure(builder, configuration.GetConnectionString(EMRSystemConsts.ConnectionStringName));

        return new EMRSystemDbContext(builder.Options);
    }
}
