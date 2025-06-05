using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace EMRSystem.EntityFrameworkCore;

public static class EMRSystemDbContextConfigurer
{
    public static void Configure(DbContextOptionsBuilder<EMRSystemDbContext> builder, string connectionString)
    {
        builder.UseSqlServer(connectionString);
    }

    public static void Configure(DbContextOptionsBuilder<EMRSystemDbContext> builder, DbConnection connection)
    {
        builder.UseSqlServer(connection);
    }
}
