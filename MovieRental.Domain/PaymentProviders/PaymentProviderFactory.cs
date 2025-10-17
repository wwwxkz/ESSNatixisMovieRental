using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using MovieRental.Domain.Interfaces;

namespace MovieRental.Domain.PaymentProviders
{
    public interface IPaymentProviderFactory
    {
        IPaymentProvider GetProvider(string providerName);
        IEnumerable<string> GetAvailableProviders();
    }

    public class PaymentProviderFactory : IPaymentProviderFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<string, Type> _providerTypes;

        public PaymentProviderFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _providerTypes = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                { "PayPal", typeof(PayPalProvider) },
                { "MBWay", typeof(MbWayProvider) },
                { "CreditCard", typeof(CreditCardProvider) }
            };
        }

        public IPaymentProvider GetProvider(string providerName)
        {
            if (providerName == null)
            {
                throw new ArgumentNullException(nameof(providerName));
            }

            if (string.IsNullOrWhiteSpace(providerName))
            {
                throw new ArgumentException("Provider name cannot be empty", nameof(providerName));
            }

            if (!_providerTypes.TryGetValue(providerName, out var providerType))
            {
                throw new InvalidOperationException($"No payment provider registered for '{providerName}'");
            }

            return (IPaymentProvider)_serviceProvider.GetRequiredService(providerType);
        }

        public IEnumerable<string> GetAvailableProviders() => _providerTypes.Keys;
    }
}
