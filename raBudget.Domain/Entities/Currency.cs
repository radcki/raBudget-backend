using System;
using System.Collections.Generic;
using System.Globalization;
using raBudget.Domain.Enums;

namespace raBudget.Domain.Entities
{
    public class Currency : IEquatable<Currency>
    {
        #region Consts

        private static Dictionary<eCurrencyCode, Currency> _currencyDictionary;

        #endregion

        #region Constructors

        private Currency()
        {
        }

        /// <summary>
        /// Constructs a currency object with a NumberFormatInfo.
        /// </summary>
        /// <param name="currencyCode"></param>
        public Currency(eCurrencyCode currencyCode)
        {
            CurrencyCode = currencyCode;
            Code = Enum.GetName(typeof(eCurrencyCode), CurrencyCode);
            var cultureInfo = CultureInfoFromCurrencyISO(Code);
            NumberFormat = cultureInfo.NumberFormat;
            var region = new RegionInfo(cultureInfo.Name);
            Symbol = region?.CurrencySymbol;
            EnglishName = region?.CurrencyEnglishName;
            NativeName = region?.CurrencyNativeName;
        }

        #endregion

        #region Properties

        public eCurrencyCode CurrencyCode { get; private set; }
        public string Code { get; private set; }
        public NumberFormatInfo NumberFormat { get; private set; }
        public string Symbol { get; private set; }
        public string EnglishName { get; private set; }
        public string NativeName { get; private set; }

        public static Dictionary<eCurrencyCode, Currency> CurrencyDictionary
        {
            get { return _currencyDictionary ??= CreateCurrencyDictionary(); }
        }

        #endregion

        #region Methods

        public static Currency Get(eCurrencyCode currencyCodeCode)
        {
            if (CurrencyDictionary.ContainsKey(currencyCodeCode))
                return CurrencyDictionary[currencyCodeCode];
            return null;
        }

        public static bool Exists(eCurrencyCode currencyCodeCode)
        {
            return CurrencyDictionary.ContainsKey(currencyCodeCode);
        }

        private static CultureInfo CultureInfoFromCurrencyISO(string isoCode)
        {
            foreach (CultureInfo ci in CultureInfo.GetCultures(CultureTypes.AllCultures))
            {
                try
                {
                    RegionInfo ri = new RegionInfo(ci.Name);
                    
                    if (ri.ISOCurrencySymbol == isoCode)
                        return ci;
                }
                catch (Exception)
                {
                }
            }

            throw new Exception("Currency code " + isoCode + " is not supported");
        }

        private static Dictionary<eCurrencyCode, Currency> CreateCurrencyDictionary()
        {
            var result = new Dictionary<eCurrencyCode, Currency>();
            foreach (eCurrencyCode code in Enum.GetValues(typeof(eCurrencyCode)))
            {
                try
                {
                    result.Add(code, new Currency(code));
                }
                catch (Exception)
                {
                }
            }

            return result;
        }

        #endregion

        #region Equality members

        /// <inheritdoc />
        public bool Equals(Currency other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return CurrencyCode == other.CurrencyCode;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((Currency) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return (int) CurrencyCode;
        }

        public static bool operator ==(Currency left, Currency right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Currency left, Currency right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}