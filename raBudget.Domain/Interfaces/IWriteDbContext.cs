using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using raBudget.Domain.Models;

namespace raBudget.Domain.Interfaces
{
    public interface IWriteDbContext
    {
        DbSet<Budget> Budgets { get; }

        DatabaseFacade Database { get; }
    }
}
