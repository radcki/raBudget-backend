using System.Text.Json.Serialization;
using FastEndpoints;

namespace raBudget.Application.Infrastructure
{
    /// <summary>Carries the initiating user's ID through in-process notifications.</summary>
    public abstract class UserNotification : IEvent
    {
        [JsonIgnore]
        public string UserId { get; internal set; }
    }

    /// <summary>Marks outgoing client handlers that run after application handlers for the same event.</summary>
    public interface IClientNotificationHandler
    {
    }
}
