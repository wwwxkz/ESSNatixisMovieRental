using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MovieRental.Domain.PaymentProviders
{
    public class PayPalProvider : BasePaymentProvider
    {
        public override string Name => "PayPal";

        public PayPalProvider(ILogger<PayPalProvider> logger) : base(logger)
        {
        }

        protected override async Task<bool> ProcessPaymentInternalAsync(decimal amount, string paymentDetails, CancellationToken cancellationToken)
        {
            try
            {
                // Simulate API call to PayPal
                await Task.Delay(100, cancellationToken);
                
                // For demo purposes, we'll simulate a 90% success rate
                return new Random().Next(10) < 9;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error processing PayPal payment");
                return false;
            }
        }
    }
}
