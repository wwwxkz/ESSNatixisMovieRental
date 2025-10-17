using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MovieRental.Domain.Interfaces;
using MovieRental.Domain.PaymentProviders;
using Xunit;

namespace MovieRental.Tests.Domain.PaymentProviders
{
    public class PaymentProviderFactoryTests
    {
        private readonly Mock<IServiceProvider> _serviceProviderMock;
        private readonly PaymentProviderFactory _factory;
        private readonly Mock<IPaymentProvider> _paypalProviderMock;
        private readonly Mock<IPaymentProvider> _mbwayProviderMock;
        private readonly Mock<IPaymentProvider> _creditCardProviderMock;

        public PaymentProviderFactoryTests()
        {
            _serviceProviderMock = new Mock<IServiceProvider>();
            _paypalProviderMock = new Mock<IPaymentProvider>();
            _mbwayProviderMock = new Mock<IPaymentProvider>();
            _creditCardProviderMock = new Mock<IPaymentProvider>();

            // Setup service provider to return our mock payment providers
            _serviceProviderMock
                .Setup(x => x.GetService(typeof(PayPalProvider)))
                .Returns(_paypalProviderMock.Object);
            
            _serviceProviderMock
                .Setup(x => x.GetService(typeof(MbWayProvider)))
                .Returns(_mbwayProviderMock.Object);
            
            _serviceProviderMock
                .Setup(x => x.GetService(typeof(CreditCardProvider)))
                .Returns(_creditCardProviderMock.Object);

            _factory = new PaymentProviderFactory(_serviceProviderMock.Object);
        }

        [Fact]
        public void GetProvider_WithValidProviderName_ReturnsCorrectProvider()
        {
            // Act
            var paypalProvider = _factory.GetProvider("PayPal");
            var mbwayProvider = _factory.GetProvider("MBWay");
            var creditCardProvider = _factory.GetProvider("CreditCard");

            // Assert
            Assert.Same(_paypalProviderMock.Object, paypalProvider);
            Assert.Same(_mbwayProviderMock.Object, mbwayProvider);
            Assert.Same(_creditCardProviderMock.Object, creditCardProvider);
        }

        [Fact]
        public void GetProvider_WithInvalidProviderName_ThrowsInvalidOperationException()
        {
            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(
                () => _factory.GetProvider("InvalidProvider"));
            
            Assert.Contains("No payment provider registered for 'InvalidProvider'", exception.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void GetProvider_WithEmptyOrWhitespaceProviderName_ThrowsArgumentException(string providerName)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(
                () => _factory.GetProvider(providerName));
            
            Assert.Equal("Provider name cannot be empty (Parameter 'providerName')", exception.Message);
        }

        [Fact]
        public void GetProvider_WithNullProviderName_ThrowsArgumentNullException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(
                () => _factory.GetProvider(null!));
            
            Assert.Equal("providerName", exception.ParamName);
        }

        [Fact]
        public void GetAvailableProviders_ReturnsAllRegisteredProviderNames()
        {
            // Act
            var providers = _factory.GetAvailableProviders();

            // Assert
            Assert.Collection(providers,
                item => Assert.Equal("PayPal", item),
                item => Assert.Equal("MBWay", item),
                item => Assert.Equal("CreditCard", item)
            );
        }
    }
}
