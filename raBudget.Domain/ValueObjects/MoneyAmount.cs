using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json.Serialization;
using raBudget.Common.Attributes;
using raBudget.Common.Interfaces;
using raBudget.Common.Resources;
using raBudget.Domain.Enums;
using raBudget.Domain.Exceptions;
using raBudget.Domain.Interfaces;
using raBudget.Domain.Models;
using RLib.Localization;

namespace raBudget.Domain.ValueObjects
{
    [SortableClass(nameof(Amount))]
    public class MoneyAmount : IEquatable<MoneyAmount>
    {
        private MoneyAmount()
        {
        }

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

        public static MoneyAmount operator +(MoneyAmount a, MoneyAmount b)
        {
            if (a.CurrencyCode != b.CurrencyCode)
            {
                throw new BusinessException(Localization.For(() => ErrorMessages.NotMatchingCurrencies));
            }

            return new MoneyAmount(a.CurrencyCode, a.Amount + b.Amount);
        }

        public static MoneyAmount operator +(MoneyAmount a, decimal b)
        {
            return new MoneyAmount(a.CurrencyCode, a.Amount + b);
        }

        public static MoneyAmount operator /(MoneyAmount a, decimal b)
        {
            return new MoneyAmount(a.CurrencyCode, a.Amount / b);
        }

        public static MoneyAmount operator *(MoneyAmount a, decimal b)
        {
            return new MoneyAmount(a.CurrencyCode, a.Amount * b);
        }

        public static MoneyAmount operator -(MoneyAmount a, MoneyAmount b)
        {
            if (a.CurrencyCode != b.CurrencyCode)
            {
                throw new BusinessException(Localization.For(() => ErrorMessages.NotMatchingCurrencies));
            }

            return new MoneyAmount(a.CurrencyCode, a.Amount - b.Amount);
        }

        public static MoneyAmount operator -(MoneyAmount a, decimal b)
        {
            return new MoneyAmount(a.CurrencyCode, a.Amount - b);
        }
    }
}