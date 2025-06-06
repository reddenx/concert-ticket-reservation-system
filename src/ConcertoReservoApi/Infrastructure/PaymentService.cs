using System;

namespace ConcertoReservoApi.Infrastructure
{
    public interface IPaymentService
    {
        record PaymentCaptureResult(string CaptureConfirmationCode, decimal AmountCaptured, bool Success, DateTime CaptureDate);

        PaymentCaptureResult CapturePayment(string id, string paymentToken, decimal totalPrice);
        bool ValidatePaymentToken(string paymentTokenizationId);
    }
}
