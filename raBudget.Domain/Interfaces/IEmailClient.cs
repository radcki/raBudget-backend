using System.Threading.Tasks;

namespace raBudget.Domain.Interfaces
{
    public interface IEmailClient
    {
        Task SendEmailAsync(string email, string subject, string htmlMessage);
    }
}
