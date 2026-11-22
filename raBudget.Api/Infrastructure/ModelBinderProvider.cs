using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using raBudget.Application.Infrastructure;

namespace raBudget.Api.Infrastructure
{
    public class ModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (typeof(IApplicationRequest).IsAssignableFrom(context.Metadata.ModelType))
            {
                return new BinderTypeModelBinder(typeof(RaBudgetModelBinder));
            }

            return null;
        }
    }
}
