using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MovieRental.Domain.PaymentProviders
{
    public class MbWayProvider : BasePaymentProvider
    {
        public override string Name => "MB Way";

        public MbWayProvider(ILogger<MbWayProvider> logger) : base(logger)
        {
        }

        protected override async Task<bool> ProcessPaymentInternalAsync(decimal amount, string paymentDetails, CancellationToken cancellationToken)
        {
            try
            {
                // Simulate API call to MB Way
                await Task.Delay(100, cancellationToken);
                
                // For demo purposes, we'll simulate a 95% success rate
                return new Random().Next(20) < 19;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error processing MB Way payment");
                return false;
            }
        }
    }
}
