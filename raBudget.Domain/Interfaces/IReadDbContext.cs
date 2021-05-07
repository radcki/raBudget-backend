using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using raBudget.Domain.Entities;
using Allocation = raBudget.Domain.ReadModels.Allocation;
using Budget = raBudget.Domain.ReadModels.Budget;
using BudgetBalance = raBudget.Domain.ReadModels.BudgetBalance;
using BudgetCategory = raBudget.Domain.ReadModels.BudgetCategory;
using BudgetCategoryBalance = raBudget.Domain.ReadModels.BudgetCategoryBalance;
using BudgetCategoryIcon = raBudget.Domain.ReadModels.BudgetCategoryIcon;
using Transaction = raBudget.Domain.ReadModels.Transaction;
using TransactionTemplate = raBudget.Domain.ReadModels.TransactionTemplate;

namespace raBudget.Domain.Interfaces
{
    public interface IReadDbContext
    {
        IQueryable<Budget> Budgets { get; }
        IQueryable<BudgetCategory> BudgetCategories { get; }
        IQueryable<Transaction> Transactions { get; }
        IQueryable<Allocation> Allocations { get; }
        IQueryable<Currency> Currencies { get; }
        IQueryable<BudgetCategoryIcon> BudgetCategoryIcons { get; }
        IQueryable<BudgetBalance> BudgetBalances { get; }
        IQueryable<BudgetCategoryBalance> BudgetCategoryBalances { get; }
        IQueryable<TransactionTemplate> TransactionTemplates { get; }
    }
}
