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
        private readonly ReadDbContext _db;

        public BudgetRepository(ReadDbContext db)
        {
            _db = db;
        }

        /// <inheritdoc />
        public Task Save(Budget entity, CancellationToken cancellationToken)
        {
            _db.Budgets.Update(entity);
            return _db.SaveChangesAsync(cancellationToken);
        }


        /// <inheritdoc />
        public async Task<Budget> GetById(Budget.Id budgetId, CancellationToken cancellationToken)
        {
            return await _db.Budgets.FindAsync(budgetId, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Budget>> GetAll(CancellationToken cancellationToken)
        {
            return await _db.Budgets.ToListAsync(cancellationToken);
        }
    }
}