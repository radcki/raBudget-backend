using System;
using System.Collections.Generic;
using System.Text;
using raBudget.Domain.Models;

namespace raBudget.Domain.ReadModels
{
    public class Budget
    {
        public int BudgetId { get; set; }
        public string Name { get; set; }
        public string OwnerUserId { get; set; }
        public DateTime StartingDate { get; set; }
        public Currency Currency { get; set; }
    }
}
