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
            var ordered = this.OrderBy(x => x.Order).ToList();
            var itemIndex = ordered.IndexOf(category);
            if (itemIndex < 0)
            {
                throw new NotFoundException(Localization.For(()=>ErrorMessages.BudgetCategoryNotFound));
            }

            if (itemIndex == ordered.Count - 1)
            {
                return;
            }

            var previous = ordered.ElementAtOrDefault(itemIndex - 1);
            category.SetOrder((previous?.Order??0)+1);
            for (var i = itemIndex+1; i < ordered.Count; i++)
            {
                ordered[i].SetOrder(ordered[i-1].Order+1);
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
            var ordered = this.OrderBy(x => x.Order).ToList();
            var itemIndex = ordered.IndexOf(category);
            if (itemIndex < 0)
            {
                throw new NotFoundException(Localization.For(() => ErrorMessages.BudgetCategoryNotFound));
            }

            if (itemIndex == 0 || ordered.Count == 1)
            {
                return;
            }

            var next = ordered.ElementAtOrDefault(itemIndex + 1);
            category.SetOrder((next?.Order ?? ordered.Count+1) - 1);
            for (var i = itemIndex - 1; i >= 0; i--)
            {
                ordered[i].SetOrder(ordered[i + 1].Order - 1);
            }
        }
    }
}