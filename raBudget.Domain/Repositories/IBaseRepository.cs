using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace raBudget.Domain.Repositories
{
    public interface IBaseRepository<in T>
    {
        Task Save(T entity, CancellationToken cancellationToken);
    }
}
