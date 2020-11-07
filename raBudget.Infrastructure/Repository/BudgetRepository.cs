using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using raBudget.Domain.Models;
using raBudget.Domain.Repositories;
using raBudget.Infrastructure.Database;

namespace raBudget.Infrastructure.Repository
{
    public class BudgetRepository : IBudgetRepository
    {
        private readonly WriteDbContext _writeDb;

        public BudgetRepository(WriteDbContext writeDb)
        {
            _writeDb = writeDb;
        }

        /// <inheritdoc />
        public Task Save(Budget entity, CancellationToken cancellationToken)
        {
            _writeDb.Budgets.Update(entity);
            return _writeDb.SaveChangesAsync(cancellationToken);
        }


        /// <inheritdoc />
        public async Task<Budget> GetById(Budget.Id budgetId, CancellationToken cancellationToken)
        {
            return await _writeDb.Budgets.FindAsync(budgetId, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Budget>> GetAll(CancellationToken cancellationToken)
        {
            return await _writeDb.Budgets.ToListAsync(cancellationToken);
        }
    }
}