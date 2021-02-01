using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
                    if (converter is CollectionConverter)
                    {
                        var emptyCollection = (IList) Activator.CreateInstance(property.PropertyType);
                        var listElementType = property.PropertyType.GetGenericArguments().Single();
                        var rowConverter = TypeDescriptor.GetConverter(listElementType);
                        foreach (var values in valueProviderResult)
                        {
                            foreach (var value in values.Split(","))
                            {
                                if (string.IsNullOrEmpty(value))
                                {
                                    continue;
                                }
                                emptyCollection.Add(rowConverter.ConvertFrom(value));
                            }
                        }

                        property.SetValue(model, emptyCollection);
                    }
                    else
                    {
                        foreach (var value in valueProviderResult)
                        {
                            property.SetValue(model, converter.ConvertFrom(value));
                        }
                    }
                }
            }


            bindingContext.Result = ModelBindingResult.Success(model);
            return Task.CompletedTask;
        }

        #endregion
    }
}