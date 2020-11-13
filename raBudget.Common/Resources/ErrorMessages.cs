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

        [Localized(KnownCulture.Polish, "")]
        public static string BudgetCategoryNotFound = "";

        [Localized(KnownCulture.Polish, "")]
        public static string SameBudgetedAmountDate = "";

        [Localized(KnownCulture.Polish, "")]
        public static string EndDateBeforeStartDate = "";
    }
}
