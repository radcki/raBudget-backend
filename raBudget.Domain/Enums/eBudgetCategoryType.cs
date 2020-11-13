using RLib.Localization.Attributes;

namespace raBudget.Domain.Enums
{
    public enum eBudgetCategoryType
    {
        [Localized("pl-PL", "Wydatek")]
        [Localized("en-GB", "Spending")]
        Spending = 1,

        [Localized("pl-PL", "Wpływ")]
        [Localized("en-GB", "Income")]
        Income = 2,

        [Localized("pl-PL", "Oszczędność")]
        [Localized("en-GB", "Saving")]
        Saving = 3
    }
}