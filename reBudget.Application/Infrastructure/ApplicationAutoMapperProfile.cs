using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoMapper;
using raBudget.Common.Interfaces;

namespace raBudget.Application.Infrastructure
{
    public class ApplicationAutoMapperProfile : Profile
    {
        public ApplicationAutoMapperProfile()
        {
            LoadCustomMappings();
            LoadConverters();
        }

        private void LoadConverters()
        {
        }

        private void LoadCustomMappings()
        {
            var mapsFrom = LoadCustomMappings(this.GetType().Assembly);

            foreach (var map in mapsFrom)
            {
                map.CreateMappings(this);
            }
        }

        private static IList<IHaveCustomMapping> LoadCustomMappings(Assembly rootAssembly)
        {
            var types = rootAssembly.GetExportedTypes();

            var mapsFrom = (
                               from type in types
                               from instance in type.GetInterfaces()
                               where
                                   typeof(IHaveCustomMapping).IsAssignableFrom(type) &&
                                   !type.IsAbstract &&
                                   !type.IsInterface
                               select (IHaveCustomMapping) Activator.CreateInstance(type)).ToList();

            return mapsFrom;
        }
    }
}