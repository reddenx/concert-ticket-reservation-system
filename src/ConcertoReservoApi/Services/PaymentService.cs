using System;
using static ConcertoReservoApi.Services.IPaymentService;

namespace ConcertoReservoApi.Services
{
    public interface IPaymentService
    {
        record PaymentCaptureResult(string CaptureConfirmationCode, decimal AmountCaptured, bool Success, DateTimeOffset CaptureDate);

        PaymentCaptureResult CapturePayment(string id, string paymentToken, decimal totalPrice);
        bool ValidatePaymentToken(string paymentTokenizationId);
    }
    public class MOCK_PAYMENT_SERVICE : IPaymentService
    {
        private readonly ITimeService _timeService;

        public MOCK_PAYMENT_SERVICE(ITimeService timeService)
        {
            _timeService = timeService;
        }

        public PaymentCaptureResult CapturePayment(string id, string paymentToken, decimal totalPrice)
        {
            return new PaymentCaptureResult(
                Guid.NewGuid().ToString("N").ToLower(),
                totalPrice,
                true,
                _timeService.GetCurrentTime());
        }

        public bool ValidatePaymentToken(string paymentTokenizationId)
        {
            return true;
        }
    }
}
