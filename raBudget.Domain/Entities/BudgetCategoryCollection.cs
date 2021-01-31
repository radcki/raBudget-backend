using System.Collections.Generic;
using System.Linq;
using raBudget.Common.Resources;
using raBudget.Domain.Exceptions;
using RLib.Localization;

namespace raBudget.Domain.Entities
{
    public class BudgetCategoryCollection: List<BudgetCategory>{
        public void MoveUp(BudgetCategory category)
        {
            var ordered = this.Where(x=>x.BudgetCategoryType == category.BudgetCategoryType)
                              .OrderBy(x => x.Order)
                              .ToList();
            var itemIndex = ordered.IndexOf(category);
            if (itemIndex < 0)
            {
                throw new NotFoundException(Localization.For(()=>ErrorMessages.BudgetCategoryNotFound));
            }

            var previous = ordered.ElementAtOrDefault(itemIndex - 1);
            if (previous != null)
            {
                ordered[itemIndex - 1] = category;
                ordered[itemIndex] = previous;
            }

            for (var i = 0; i < ordered.Count; i++)
            {
                ordered[i].SetOrder(i);
            }
        }

        public new void Add(BudgetCategory item)
        {
            if (this.Count > 0)
            {
                item.SetOrder(this.Max(x => x.Order)+1);
            }
            base.Add(item);
        }

        public void MoveDown(BudgetCategory category)
        {
            var ordered = this.Where(x => x.BudgetCategoryType == category.BudgetCategoryType)
                              .OrderBy(x => x.Order)
                              .ToList();
            var itemIndex = ordered.IndexOf(category);
            if (itemIndex < 0)
            {
                throw new NotFoundException(Localization.For(() => ErrorMessages.BudgetCategoryNotFound));
            }

            var next = ordered.ElementAtOrDefault(itemIndex + 1);
            if (next != null)
            {
                ordered[itemIndex + 1] = category;
                ordered[itemIndex] = next;
            }

            for (var i = 0; i < ordered.Count; i++)
            {
                ordered[i].SetOrder(i);
            }
        }
    }
}