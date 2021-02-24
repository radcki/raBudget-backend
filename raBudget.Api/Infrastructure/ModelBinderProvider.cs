using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.Internal;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using raBudget.Common.Query;

namespace raBudget.Api.Infrastructure
{
    public class ModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ModelType.ImplementsGenericInterface(typeof(IRequest<>)))
            {
                return new BinderTypeModelBinder(typeof(RaBudgetModelBinder));
            }

            return null;
        }
    }
}
