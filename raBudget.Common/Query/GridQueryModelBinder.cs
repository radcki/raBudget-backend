using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace raBudget.Common.Query
{
    public class GridQueryModelBinder : IModelBinder
    {
        #region Implementation of IModelBinder

        /// <inheritdoc />
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var dataOrder = new FieldOrderInfoCollection();

            var model = Activator.CreateInstance(bindingContext.ModelType);
            var properties = bindingContext.ModelType.GetProperties();

            foreach (var property in properties)
            {
                var valueProviderResult = bindingContext.ValueProvider.GetValue(property.Name);
                if (property.PropertyType == typeof(FieldOrderInfoCollection))
                {
                    foreach (var dataOrderField in valueProviderResult)
                    {
                        dataOrder.Add(new FieldOrderInfo()
                                      {
                                          Descending = dataOrderField.StartsWith("-"),
                                          FieldName = dataOrderField.TrimStart('-', '+')
                                      });
                    }

                    property.SetValue(model, dataOrder);
                }
                else
                {
                    var converter = TypeDescriptor.GetConverter(property.PropertyType);
                    foreach (var value in valueProviderResult)
                    {
                        property.SetValue(model, converter.ConvertFrom(value));
                    }
                }
            }


            bindingContext.Result = ModelBindingResult.Success(model);
            return Task.CompletedTask;
        }

        #endregion
    }
}