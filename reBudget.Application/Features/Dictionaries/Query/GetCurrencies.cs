using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using raBudget.Common.Interfaces;
using raBudget.Common.Response;
using raBudget.Domain.Entities;
using raBudget.Domain.Enums;
using raBudget.Domain.Interfaces;

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
            private readonly IMapper _mapper;
            private readonly MapperConfiguration _mapperConfiguration;

            public Handler(IReadDbContext readDb, MapperConfiguration mapperConfiguration, IMapper mapper)
            {
                _readDb = readDb;
                _mapperConfiguration = mapperConfiguration;
                _mapper = mapper;
            }

            public async Task<Result> Handle(Query request, CancellationToken cancellationToken)
            {
                var currencies = await _readDb.Currencies.ToListAsync(cancellationToken);
                var data = _mapper.Map<List<CurrencyDto>>(currencies).ToList();
                return new Result()
                       {
                           Data = data,
                           Total = data.Count()
                       };
            }
        }
    }
}