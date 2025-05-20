namespace MovieRental.PaymentProviders
{
    public class PayPalProvider
    {
        public Task<bool> Pay(double price)
        {
            //ignore this implementation
            return Task.FromResult<bool>(true);
        }
    }
}
