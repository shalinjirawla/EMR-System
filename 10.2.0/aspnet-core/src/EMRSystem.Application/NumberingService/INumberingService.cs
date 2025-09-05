using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.NumberingService
{
    public interface INumberingService
    {
        Task<string> GenerateReceiptNumberAsync<TEntity>(
            IRepository<TEntity, long> repository,
            string prefix,
            int tenantId,
            string propertyName
        ) where TEntity : class, IEntity<long>;
    }
}
