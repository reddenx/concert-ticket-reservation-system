namespace ConcertoReservoApi.Infrastructure
{
    public interface IPaymentService
    {
        record PaymentCaptureResult(string CaptureConfirmationCode, decimal AmountCaptured, bool Success);

        PaymentCaptureResult CapturePayment(string id, string paymentToken, decimal totalPrice);
        bool ValidatePaymentToken(string paymentTokenizationId);
    }
}
