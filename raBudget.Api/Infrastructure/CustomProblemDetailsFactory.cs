using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using raBudget.Domain.Exceptions;

namespace raBudget.Api.Infrastructure
{
    public class CustomProblemDetailsFactory : ProblemDetailsFactory
    {
        #region Overrides of ProblemDetailsFactory

        /// <inheritdoc />
        public override ProblemDetails CreateProblemDetails(HttpContext httpContext, int? statusCode = null, string title = null, string type = null, string detail = null, string instance = null)
        {
            var context = httpContext.Features.Get<IExceptionHandlerFeature>();

            if (context?.Error != null)
            {
                var exception = context.Error;
                if (exception is NotFoundException notFoundException)
                {
                    detail = notFoundException.Details;
                    statusCode = (int) HttpStatusCode.NotFound;
                }
                else if (exception is BusinessException businessException)
                {
                    detail = businessException.Details;
                    statusCode = (int) HttpStatusCode.InternalServerError;
                }
            }

            return new ProblemDetails()
                   {
                       Detail = detail,
                       Status = statusCode,
                   };
        }

        /// <inheritdoc />
        public override ValidationProblemDetails CreateValidationProblemDetails(HttpContext httpContext, ModelStateDictionary modelStateDictionary, int? statusCode = null, string title = null, string type = null, string detail = null, string instance = null)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}