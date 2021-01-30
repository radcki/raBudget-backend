using System;
using raBudget.Common.Entities;
using raBudget.Common.Resources;
using raBudget.Domain.Enums;
using raBudget.Domain.Exceptions;
using raBudget.Domain.ValueObjects;
using RLib.Localization;

namespace raBudget.Domain.Entities
{
    public class SubTransaction : BaseEntity
    {
        private SubTransaction()
        {
        }

        public static SubTransaction Create
        (
            Transaction parent,
            string description,
            MoneyAmount amount,
            DateTime transactionDate)
        {
            var subTransaction = new SubTransaction
                                 {
                                     ParentTransactionId = parent.TransactionId,
                                     SubTransactionId = new SubTransactionId(),
                                     Description = description
                                 };
            subTransaction.SetAmount(amount);
            subTransaction.SetTransactionDate(transactionDate);
            subTransaction.CreationDateTime = DateTime.Now;

            return subTransaction;
        }

        public SubTransactionId SubTransactionId { get; private set; }
        public TransactionId ParentTransactionId { get; private set; }
        public string Description { get; private set; } 
        public MoneyAmount Amount { get; private set; }
        public DateTime TransactionDate { get; private set; }
        public DateTime CreationDateTime { get; private set; }

        public void SetAmount(MoneyAmount newAmount)
        {
            if (Amount != null && newAmount.Currency != Amount.Currency)
            {
                throw new BusinessException("New amount must be of same currency");
            }

            Amount = newAmount;
        }

        public void SetDescription(string description)
        {
            description = description.Trim();
            if (string.IsNullOrEmpty(description))
            {
                throw new BusinessException(Localization.For(() => ErrorMessages.TransactionDescriptionEmpty));
            }

            Description = description;
        }

        public void SetTransactionDate(DateTime newTransactionDate)
        {
            TransactionDate = newTransactionDate.Date;
        }

        
    }
}