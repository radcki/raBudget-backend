using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using raBudget.Domain.Models;
using raBudget.Domain.ReadModels;

namespace raBudget.Domain.Interfaces
{
    public interface IReadDbContext
    {
        IQueryable<Budget> Budgets { get; }
        IQueryable<BudgetCategory> BudgetCategories { get; }
        IQueryable<Transaction> Transactions { get; }
        IQueryable<Currency> Currencies { get; }
        IQueryable<BudgetCategoryIcon> BudgetCategoryIcons { get; }
    }
}
