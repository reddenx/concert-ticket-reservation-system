namespace ConcertoReservoApi.Infrastructure
{
    public interface IPaymentService
    {
        string CapturePayment(string id, string paymentToken, decimal totalPrice);
        bool ValidatePaymentToken(string paymentTokenizationId);
    }
}
