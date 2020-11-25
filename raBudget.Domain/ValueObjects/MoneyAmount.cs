using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using raBudget.Domain.Enums;
using raBudget.Domain.Models;

namespace raBudget.Domain.ValueObjects
{
    public class MoneyAmount : IEquatable<MoneyAmount>
    {
        private MoneyAmount(){}
        public MoneyAmount(Currency currency, decimal amount)
        {
            this.CurrencyCode = currency.CurrencyCode;
            this.Amount = amount;
        }

        public eCurrencyCode CurrencyCode { get; set; }
        public Currency Currency => new Currency(CurrencyCode);
        public decimal Amount { get; private set; }

        public bool Equals(MoneyAmount other)
        {
            if (object.ReferenceEquals(other, null)) return false;
            if (object.ReferenceEquals(other, this)) return true;
            return this.Currency.Equals(other.Currency) && this.Amount.Equals(other.Amount);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as MoneyAmount);
        }

        public override int GetHashCode()
        {
            return this.Currency.GetHashCode() ^ this.Amount.GetHashCode();
        }
    }
}