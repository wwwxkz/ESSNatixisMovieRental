using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MovieRental.Domain.PaymentProviders
{
    public class CreditCardProvider : BasePaymentProvider
    {
        public override string Name => "CreditCard";

        public CreditCardProvider(ILogger<CreditCardProvider> logger) : base(logger)
        {
        }

        protected override async Task<bool> ProcessPaymentInternalAsync(decimal amount, string paymentDetails, CancellationToken cancellationToken)
        {
            try
            {
                // Simulate API call to credit card processor
                await Task.Delay(150, cancellationToken);
                
                // For demo purposes, we'll simulate a 95% success rate
                return new Random().Next(20) < 19;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error processing credit card payment");
                return false;
            }
        }
    }
}
