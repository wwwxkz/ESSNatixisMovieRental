using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using MovieRental.Domain.Interfaces;
using MovieRental.Domain.PaymentProviders;

namespace MovieRental.Tests.Helpers
{
    public static class MockPaymentProviderFactory
    {
        public static Mock<IPaymentProviderFactory> Create()
        {
            var mockFactory = new Mock<IPaymentProviderFactory>();
            var mockProvider = new Mock<IPaymentProvider>();
            
            mockProvider.Setup(p => p.Name).Returns("TestProvider");
            mockProvider.Setup(p => p.ProcessPaymentAsync(It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
                
            mockFactory.Setup(f => f.GetProvider(It.IsAny<string>()))
                .Returns(mockProvider.Object);
                
            mockFactory.Setup(f => f.GetAvailableProviders())
                .Returns(new[] { "CreditCard", "PayPal", "MBWay" });
                
            return mockFactory;
        }
    }
}
