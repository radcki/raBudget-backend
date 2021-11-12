using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using raBudget.Common.Query;
using raBudget.Domain.ValueObjects;

namespace raBudget.Api.Infrastructure
{
    public class RaBudgetModelBinder : IModelBinder
    {
        #region Implementation of IModelBinder

        /// <inheritdoc />
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext.BindingSource == BindingSource.Body)
            {
                string json;
                using (var reader = new StreamReader(bindingContext.ActionContext.HttpContext.Request.Body, Encoding.UTF8))
                    json = reader.ReadToEndAsync().GetAwaiter().GetResult();
                var bodyModel = JsonSerializer.Deserialize(json, bindingContext.ModelType, new JsonSerializerOptions()
                                                                                           {
                                                                                               PropertyNameCaseInsensitive = true
                                                                                           });
                //var bodyModel = JsonConvert.DeserializeObject(json, bindingContext.ModelType);
                bindingContext.Result = ModelBindingResult.Success(bodyModel);
                return Task.CompletedTask;
            }

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