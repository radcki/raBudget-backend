﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using raBudget.Domain.Entities;
using raBudget.Domain.Enums;
using raBudget.Domain.Interfaces;
using raBudget.Domain.ValueObjects;
using BudgetId = raBudget.Domain.ValueObjects.BudgetId;

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

        public IEnumerable<BudgetId> GetAccessibleBudgetIds()
        {
            return _readDbContext.Budgets.Where(x => x.OwnerUserId == _userContext.UserId).Select(x => x.BudgetId);
        }

        public async Task<bool> HasBudgetAccessAsync(BudgetId budgetBudgetId)
        {
            return await _readDbContext.Budgets.AnyAsync(x => x.OwnerUserId == _userContext.UserId && x.BudgetId == budgetBudgetId);
        }

        public IEnumerable<BudgetCategoryId> GetAccessibleBudgetCategoryIds(BudgetId budgetId, eBudgetCategoryType? budgetCategoryType = null)
        {
            var query = _readDbContext.BudgetCategories.Where(x => x.BudgetId == budgetId && GetAccessibleBudgetIds().Any(s=>s == x.BudgetId));
            if (budgetCategoryType != null)
            {
                query = query.Where(x => x.BudgetCategoryType == budgetCategoryType);
            }
            return query.Select(x => x.BudgetCategoryId);
        }

        public async Task<bool> HasBudgetCategoryAccessAsync(BudgetCategoryId budgetCategoryBudgetCategoryId)
        {
            return await _readDbContext.BudgetCategories
                                       .AnyAsync(x => x.BudgetCategoryId == budgetCategoryBudgetCategoryId
                                                      && _readDbContext.Budgets
                                                                       .Any(b => b.BudgetId == x.BudgetId && b.OwnerUserId == _userContext.UserId));
        }

        public async Task<bool> HasBudgetCategoriesAccessAsync(IEnumerable<BudgetCategoryId> budgetCategoryBudgetCategoryIds)
        {
            return await _readDbContext.BudgetCategories
                                       .AnyAsync(x => budgetCategoryBudgetCategoryIds.Any(s=>s==x.BudgetCategoryId)
                                                      && _readDbContext.Budgets
                                                                       .Any(b => b.BudgetId == x.BudgetId && b.OwnerUserId == _userContext.UserId));
        }


        public async Task<bool> HasTransactionAccess(TransactionId transactionTransactionId)
        {
            return await _readDbContext.Transactions
                                       .AnyAsync(x => x.TransactionId == transactionTransactionId
                                                      && _readDbContext.BudgetCategories
                                                                       .Any(s => x.BudgetCategoryId == s.BudgetCategoryId
                                                                                 && _readDbContext.Budgets
                                                                                                  .Any(b => b.BudgetId == s.BudgetId && b.OwnerUserId == _userContext.UserId)));
        }

        public async Task<bool> HasTransactionTemplateAccess(TransactionTemplateId transactionTemplateId)
        {
            return await _readDbContext.TransactionTemplates
                                       .AnyAsync(x => x.TransactionTemplateId == transactionTemplateId
                                                      && _readDbContext.BudgetCategories
                                                                       .Any(s => x.BudgetCategoryId == s.BudgetCategoryId
                                                                                 && _readDbContext.Budgets
                                                                                                  .Any(b => b.BudgetId == s.BudgetId && b.OwnerUserId == _userContext.UserId)));
        }

        public async Task<bool> HasAllocationAccess(AllocationId allocationId)
		{
			return await _readDbContext.Allocations
									   .AnyAsync(x => x.AllocationId == allocationId
													  && _readDbContext.BudgetCategories
																	   .Any(s => x.TargetBudgetCategoryId == s.BudgetCategoryId
																				 && _readDbContext.Budgets
																								  .Any(b => b.BudgetId == s.BudgetId && b.OwnerUserId == _userContext.UserId)));
		}
    }
}