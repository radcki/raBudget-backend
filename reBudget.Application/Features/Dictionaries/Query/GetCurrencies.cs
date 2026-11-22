using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using raBudget.Application.Infrastructure;
using raBudget.Common.Response;
using raBudget.Domain.Entities;
using raBudget.Domain.Enums;
using raBudget.Domain.Interfaces;

namespace raBudget.Application.Features.Dictionaries.Query
{
    public class GetCurrencies
    {
        public class Query : IApplicationRequest<Result>
        {
        }

        public class Result : CollectionResponse<CurrencyDto>
        {
        }

        public class CurrencyDto
        {
            /// <summary>Creates a DTO using the currency's culture-derived display information.</summary>
            internal static CurrencyDto CreateFrom(Currency currency)
            {
                return new CurrencyDto
                {
                    CurrencyCode = currency.CurrencyCode,
                    Code = currency.Code,
                    NumberFormat = currency.NumberFormat,
                    Symbol = currency.Symbol,
                    EnglishName = currency.EnglishName,
                    NativeName = currency.NativeName
                };
            }

            public eCurrencyCode CurrencyCode { get; set; }
            public string Code { get; private set; }
            public NumberFormatInfo NumberFormat { get; set; }
            public string Symbol { get; private set; }
            public string EnglishName { get; private set; }
            public string NativeName { get; private set; }
        }

        public class Handler : ICommandHandler<Query, Result>
        {
            private readonly IReadDbContext _readDb;

            public Handler(IReadDbContext readDb)
            {
                _readDb = readDb;
            }

            public async Task<Result> ExecuteAsync(Query request, CancellationToken cancellationToken)
            {
                var data = await _readDb.Currencies.ToCurrencyDtos().ToListAsync(cancellationToken);
                return new Result()
                       {
                           Data = data,
                           Total = data.Count
                       };
            }
        }
    }
}
