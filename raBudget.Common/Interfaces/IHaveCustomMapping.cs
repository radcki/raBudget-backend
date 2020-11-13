using AutoMapper;

namespace raBudget.Common.Interfaces
{
    public interface IHaveCustomMapping
    {
        void CreateMappings(Profile configuration);
    }
}