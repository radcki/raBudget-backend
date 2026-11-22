using FastEndpoints;

namespace raBudget.Application.Infrastructure
{
    /// <summary>Identifies DTOs that use the application's MVC model binding conventions.</summary>
    public interface IApplicationRequest
    {
    }

    /// <summary>Defines an application command or query that returns a result.</summary>
    public interface IApplicationRequest<TResult> : IApplicationRequest, ICommand<TResult>
    {
    }
}
