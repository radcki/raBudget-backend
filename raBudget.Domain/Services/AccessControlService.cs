using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using raBudget.Domain.Entities;
using raBudget.Domain.Interfaces;

namespace raBudget.Domain.Services
{
    public class AccessControlService
    {
        private readonly IUserContext _userContext;
        private readonly IReadDbContext _readDbContext;

        public AccessControlService(IUserContext userContext, IReadDbContext readDbContext)
        {
            _userContext = userContext;
            _readDbContext = readDbContext;
        }

        public IEnumerable<Budget.Id> GetAccessibleBudgetIds()
        {
            return _readDbContext.Budgets.Where(x => x.OwnerUserId == _userContext.UserId).Select(x => x.BudgetId);
        }

        public async Task<bool> HasBudgetAccessAsync(Budget.Id budgetId)
        {
            return await _readDbContext.Budgets.AnyAsync(x => x.OwnerUserId == _userContext.UserId && x.BudgetId == budgetId);
        }

        public IEnumerable<BudgetCategory.Id> GetAccessibleBudgetCategoryIds()
        {
            return _readDbContext.BudgetCategories.Where(x => GetAccessibleBudgetIds().Contains(x.BudgetId)).Select(x => x.BudgetCategoryId);
        }

        public async Task<bool> HasBudgetCategoryAccessAsync(BudgetCategory.Id budgetCategoryId)
        {
            return await _readDbContext.BudgetCategories
                                       .AnyAsync(x => x.BudgetCategoryId == budgetCategoryId
                                                      && _readDbContext.Budgets
                                                                       .Any(b => b.BudgetId == x.BudgetId && b.OwnerUserId == _userContext.UserId));
        }


        public async Task<bool> HasTransactionAccess(Transaction.Id transactionId)
        {
            return await _readDbContext.Transactions
                                       .AnyAsync(x => x.TransactionId == transactionId
                                                      && _readDbContext.BudgetCategories
                                                                       .Any(s => x.BudgetCategoryId == s.BudgetCategoryId
                                                                                 && _readDbContext.Budgets
                                                                                                  .Any(b => b.BudgetId == s.BudgetId && b.OwnerUserId == _userContext.UserId)));
        }
    }
}