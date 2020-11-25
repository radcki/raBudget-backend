using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using raBudget.Common.Interfaces;
using raBudget.Common.Response;
using raBudget.Domain.Enums;
using raBudget.Domain.Interfaces;
using raBudget.Domain.Models;

namespace raBudget.Application.Features.Dictionaries.Query
{
    public class GetCurrencies
    {
        public class Query : IRequest<Result>
        {
        }

        public class Result : CollectionResponse<CurrencyDto>
        {
        }

        public class CurrencyDto
        {
            public eCurrencyCode CurrencyCode { get; set; }
            public string Code { get; private set; }
            public NumberFormatInfo NumberFormat { get; set; }
            public string Symbol { get; private set; }
            public string EnglishName { get; private set; }
            public string NativeName { get; private set; }
        }

        public class Mapper : IHaveCustomMapping
        {
            public void CreateMappings(Profile configuration)
            {
                configuration.CreateMap<Currency, CurrencyDto>();
            }
        }

        public class Handler : IRequestHandler<Query, Result>
        {
            private readonly IReadDbContext _readDb;
            private readonly MapperConfiguration _mapperConfiguration;

            public Handler(IReadDbContext readDb, MapperConfiguration mapperConfiguration)
            {
                _readDb = readDb;
                _mapperConfiguration = mapperConfiguration;
            }

            public async Task<Result> Handle(Query request, CancellationToken cancellationToken)
            {
                var data = _readDb.Currencies.ProjectTo<CurrencyDto>(_mapperConfiguration);

                return new Result()
                       {
                           Data = await data.ToListAsync(cancellationToken),
                           Total = data.Count()
                       };
            }
        }
    }
}