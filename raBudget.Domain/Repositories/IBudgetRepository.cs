using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using raBudget.Domain.Models;

namespace raBudget.Domain.Repositories
{
    public interface IBudgetRepository : IBaseRepository<Budget>
    {
        Task<Budget> GetById(Budget.Id budgetId, CancellationToken cancellationToken);
        
        Task<IEnumerable<Budget>> GetAll(CancellationToken cancellationToken);
    }
}
