using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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


        public async Task<bool> HasBudgetAccessAsync(int budgetId)
        {
            return await _readDbContext.Budgets.AnyAsync(x => x.OwnerUserId == _userContext.UserId && x.BudgetId == budgetId);
        }

        public async Task<bool> HasBudgetCategoryAccessAsync(int budgetCategoryId)
        {
            return await _readDbContext.BudgetCategories
                                       .AnyAsync(x => x.BudgetCategoryId == budgetCategoryId
                                                      && _readDbContext.Budgets
                                                                       .Any(b => b.BudgetId == x.BudgetId && b.OwnerUserId == _userContext.UserId));
        }


        public async Task<bool> HasTransactionAccess(int transactionId)
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