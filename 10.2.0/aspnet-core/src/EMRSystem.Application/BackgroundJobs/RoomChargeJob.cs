using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.EntityFrameworkCore;
using EMRSystem.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace EMRSystem.BackgroundJobs
{
    public class RoomChargeJob : IRoomChargeJob, ITransientDependency
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IDbContextProvider<EMRSystemDbContext> _dbContextProvider;
        private readonly ILogger<RoomChargeJob> _logger;

        public RoomChargeJob(
            IUnitOfWorkManager unitOfWorkManager,
             ILogger<RoomChargeJob> logger,
            IDbContextProvider<EMRSystemDbContext> dbContextProvider)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _logger = logger;
            _dbContextProvider = dbContextProvider;
        }

        public async Task ExecuteAsync()
        {
            try
            {
                using (var uow = _unitOfWorkManager.Begin())
                {
                    var dbContext = await _dbContextProvider.GetDbContextAsync();
                    await dbContext.Database.ExecuteSqlRawAsync("EXEC [dbo].[AddDailyRoomCharges]");

                    await uow.CompleteAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AddDailyRoomCharges job failed at {Time}", DateTime.Now);
                throw;
            }
        }
    }
}
