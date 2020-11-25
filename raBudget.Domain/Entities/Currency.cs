using System;
using System.Collections.Generic;
using System.Globalization;
using raBudget.Domain.Enums;

namespace raBudget.Domain.Models
{
    public class Currency
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
            var region = new RegionInfo(cultureInfo.LCID);
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
            foreach (CultureInfo ci in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
            {
                try
                {
                    RegionInfo ri = new RegionInfo(ci.LCID);
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
    }
}