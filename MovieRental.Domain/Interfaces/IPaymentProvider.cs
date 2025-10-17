using System.Threading;
using System.Threading.Tasks;

namespace MovieRental.Domain.Interfaces
{
    public interface IPaymentProvider
    {
        string Name { get; }
        Task<bool> ProcessPaymentAsync(decimal amount, string paymentDetails, CancellationToken cancellationToken = default);
    }
}
