using System;
using System.Collections.Generic;
using System.Text;
using RLib.Localization.Attributes;

namespace raBudget.Common.Resources
{
    public class ErrorMessages
    {
        [Localized(KnownCulture.Polish, "Użytkownik musi być zdefiniowany")]
        public static string UserRequired = "User must be specified";

        [Localized(KnownCulture.Polish, "Budżet nie został odnaleziony")]
        public static string BudgetNotFound = "Budget was not found";

        [Localized(KnownCulture.Polish, "Kategoria budżetowa musi być tego samego typu")]
        public static string NotSameBudgetCategoryType = "Budget category must be of same type";

        [Localized(KnownCulture.Polish, "Kategoria budżetowa nie została odnaleziona")]
        public static string BudgetCategoryNotFound = "";

        [Localized(KnownCulture.Polish, "Transakcja nie została odnaleziona")]
        public static string TransactionNotFound = "";

        [Localized(KnownCulture.Polish, "Schemat transakcji nie został odnaleziony")]
        public static string TransactionTemplateNotFound = "";

        [Localized(KnownCulture.Polish, "Alokacja nie została odnaleziona")]
		public static string AllocationNotFound = "";

        [Localized(KnownCulture.Polish, "Opis transakcji nie może być pusty")]
        public static string TransactionDescriptionEmpty = "";

        [Localized(KnownCulture.Polish, "Kategoria budżetowa nie została wybrana")]
        public static string BudgetCategoryEmpty = "";

        [Localized(KnownCulture.Polish, "Nie odnaleziono zabudżetowanej kwoty")]
        public static string BudgetedAmountNotFound = "";

        [Localized(KnownCulture.Polish, "Już istnieje zabudżetowana kwota z tą samą datą początku obowiązywania")]
        public static string SameBudgetedAmountDate = "";

        [Localized(KnownCulture.Polish, "Data końca musi być późniejsza niż data początku")]
        public static string EndDateBeforeStartDate = "";

		[Localized(KnownCulture.Polish, "Waluty muszą być zgodne")]
		public static string NotMatchingCurrencies = "";
    }
}
