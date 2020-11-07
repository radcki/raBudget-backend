using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using raBudget.Domain.Models;

namespace raBudget.Domain.Repositories
{
    public interface ITransactionRepository : IBaseRepository<Transaction>
    {
        Task<Transaction> GetById(Transaction.Id budgetId, CancellationToken cancellationToken);
        
    }
}
