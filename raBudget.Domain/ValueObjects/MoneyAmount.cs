using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json.Serialization;
using raBudget.Domain.Enums;
using raBudget.Domain.Models;

namespace raBudget.Domain.ValueObjects
{
    public class MoneyAmount : IEquatable<MoneyAmount>
    {
        private MoneyAmount(){}
        public MoneyAmount(eCurrencyCode currencyCode, decimal amount)
        {
            this.CurrencyCode = currencyCode;
            this.Amount = amount;
        }

        public eCurrencyCode CurrencyCode { get; set; }
        [JsonIgnore] public Currency Currency => new Currency(CurrencyCode);
        public decimal Amount { get; private set; }

        public bool Equals(MoneyAmount other)
        {
            if (object.ReferenceEquals(other, null)) return false;
            if (object.ReferenceEquals(other, this)) return true;
            return this.CurrencyCode.Equals(other.CurrencyCode) && this.Amount.Equals(other.Amount);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as MoneyAmount);
        }

        public override int GetHashCode()
        {
            return this.Amount.GetHashCode();
        }
    }
}