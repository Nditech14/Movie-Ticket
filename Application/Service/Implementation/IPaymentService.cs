using Core.Entities;

namespace Application.Service.Implementation
{
    public interface IPaymentService
    {
        Task<PaymentResult> CreatePaymentForUserAsync();
        Task<PaymentResult> VerifyPaymentAsync(string reference);
    }
}