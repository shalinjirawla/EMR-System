using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EMRSystem.NumberingService
{
    public class NumberingService : INumberingService, ITransientDependency
    {
        public async Task<string> GenerateReceiptNumberAsync<TEntity>(
            IRepository<TEntity, long> repository,
            string prefix,
            int tenantId,
            string propertyName // <- yaha pass karna hai ReceiptNo / InvoiceNo
        ) where TEntity : class, IEntity<long>
        {
            var year = DateTime.Now.Year;
            var searchPrefix = $"{prefix}-{year}";

            // last record nikalna
            var lastEntity = await repository.GetAll()
                .Where(x => EF.Property<int>(x, "TenantId") == tenantId)
                .Where(x => EF.Property<string>(x, propertyName) != null &&
                            EF.Property<string>(x, propertyName).StartsWith(searchPrefix))
                .OrderByDescending(x => x.Id) // safer than string order
                .FirstOrDefaultAsync();

            int sequence = 1;
            if (lastEntity != null)
            {
                var lastNumber = (string)lastEntity.GetType()
                    .GetProperty(propertyName)
                    .GetValue(lastEntity);

                if (!string.IsNullOrEmpty(lastNumber))
                {
                    var parts = lastNumber.Split('-');
                    if (int.TryParse(parts.Last(), out int lastSeq))
                        sequence = lastSeq + 1;
                }
            }

            return $"{prefix}-{year}-{sequence.ToString().PadLeft(6, '0')}";
        }
    }
}
