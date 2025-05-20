namespace MovieRental.PaymentProviders
{
    public class MbWayProvider
    {
        public Task<bool> Pay(double price)
        {
            //ignore this implementation
            return Task.FromResult<bool>(true);
        }
    }
}
