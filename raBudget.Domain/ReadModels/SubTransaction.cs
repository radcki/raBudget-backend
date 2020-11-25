using System;
using raBudget.Domain.ValueObjects;

namespace raBudget.Domain.ReadModels
{
    public class SubTransaction
    {
        public SubTransactionId SubTransactionId { get; set; }
        public TransactionId ParentTransactionTransactionId { get; set; }
        public string Description { get; set; }
        public MoneyAmount Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime CreationDateTime { get; set; }
    }
}