using System;
using System.Collections.Generic;
using System.Text;
using raBudget.Common.Entities;
using raBudget.Domain.Enums;
using raBudget.Domain.Models;
using raBudget.Domain.ValueObjects;

namespace raBudget.Domain.ReadModels
{
    public class TransactionTemplate
    {
        public TransactionTemplateId TransactionTemplateId { get; set; }
        public string Description { get; set; }
        public BudgetCategoryId BudgetCategoryId { get; set; }
        public MoneyAmount Amount { get; set; }
        public DateTime CreationDateTime { get; set; }
    }
}