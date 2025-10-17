using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MovieRental.Domain.Interfaces;

namespace MovieRental.Domain.PaymentProviders
{
    public abstract class BasePaymentProvider : IPaymentProvider
    {
        protected readonly ILogger Logger;

        protected BasePaymentProvider(ILogger logger)
        {
            Logger = logger;
        }

        public abstract string Name { get; }

        public async Task<bool> ProcessPaymentAsync(decimal amount, string paymentDetails, CancellationToken cancellationToken = default)
        {
            Logger.LogInformation($"Processing {Name} payment of {amount:C}");
            
            // Simulate payment processing
            await Task.Delay(100, cancellationToken);
            
            // In a real application, this would call the actual payment provider's API
            bool isSuccessful = await ProcessPaymentInternalAsync(amount, paymentDetails, cancellationToken);
            
            if (isSuccessful)
            {
                Logger.LogInformation($"{Name} payment processed successfully");
            }
            else
            {
                Logger.LogWarning($"{Name} payment failed");
            }
            
            return isSuccessful;
        }

        protected abstract Task<bool> ProcessPaymentInternalAsync(decimal amount, string paymentDetails, CancellationToken cancellationToken);
    }
}
