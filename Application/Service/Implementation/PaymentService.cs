using Application.Service.Abstraction;
using Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Application.Service.Implementation
{
    public class PaymentService : IPaymentService
    {
        private readonly PayStackSettings _payStackSettings;
        private readonly ICartService _cartService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<PaymentService> _logger;
        private readonly HttpClient _httpClient;
        private readonly IEmailService _emailService;

        public PaymentService(IOptions<PayStackSettings> payStackSettings,
            ICartService cartService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<PaymentService> logger,
            HttpClient httpClient,
            IEmailService emailService)
        {
            _payStackSettings = payStackSettings.Value;
            _cartService = cartService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _httpClient = httpClient;
            _emailService = emailService;
        }

        private string GetUserId()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogError("User is not authenticated.");
                throw new UnauthorizedAccessException("User is not authenticated.");
            }
            return userId;
        }

        public async Task<PaymentResult> CreatePaymentForUserAsync()
        {
            try
            {
                var userId = GetUserId();
                var cartResponse = await _cartService.GetCartAsync();
                if (!cartResponse.IsSuccessful)
                {
                    _logger.LogError($"Failed to retrieve cart for user {userId}. Error: {cartResponse.Message}");
                    return new PaymentResult { Success = false, Message = cartResponse.Message };
                }

                var cart = cartResponse.Data;
                var totalAmount = cart.CartItems.Sum(item => item.Quantity * item.Price);

                return await CreatePaymentAsync(userId, totalAmount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during payment creation.");
                return new PaymentResult { Success = false, Message = "An error occurred while creating the payment." };
            }
        }

        private async Task<PaymentResult> CreatePaymentAsync(string userId, decimal amount)
        {

            var email = _httpContextAccessor.HttpContext.User.Claims
    .FirstOrDefault(c => c.Type == "email" || c.Type == "emails")?.Value;
            var requestBody = new
            {
                email = email,
                amount = (int)(amount * 100),
                callback_url = $"http://localhost:7059/api/Payment/verify-payment?"
            };

            var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");


            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _payStackSettings.SecretKey);

            try
            {

                _logger.LogInformation($"Initializing payment for user {userId}, Amount: {amount}, Email: {email}");


                var response = await _httpClient.PostAsync($"{_payStackSettings.BaseUrl}/transaction/initialize", content);
                var responseContent = await response.Content.ReadAsStringAsync();


                _logger.LogInformation($"Paystack Response for user {userId}: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    var paystackResponse = JsonConvert.DeserializeObject<PaystackResponse>(responseContent);
                    if (paystackResponse?.data?.authorization_url != null)
                    {

                        return new PaymentResult
                        {
                            Success = true,
                            RedirectUrl = paystackResponse.data.authorization_url,
                            Message = "Payment initialized. Redirect to Paystack for approval."
                        };
                    }
                    else
                    {

                        _logger.LogError($"Unexpected response structure: {responseContent}");
                        return new PaymentResult { Success = false, Message = "Unexpected response from Paystack." };
                    }
                }
                else
                {

                    _logger.LogError($"Failed to initialize payment for user {userId}. Response: {responseContent}");
                    return new PaymentResult { Success = false, Message = "Failed to initialize payment with Paystack." };
                }
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, $"Error creating Paystack payment for user {userId}.");
                return new PaymentResult { Success = false, Message = "An error occurred while creating the Paystack payment." };
            }
        }
        public async Task<PaymentResult> VerifyPaymentAsync(string reference)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _payStackSettings.SecretKey);
                var response = await _httpClient.GetAsync($"{_payStackSettings.BaseUrl}/transaction/verify/{reference}");
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var verificationResponse = JsonConvert.DeserializeObject<PaystackResponse>(responseContent);

                    if (verificationResponse.data.status == "success")
                    {
                        var cartResponse = await _cartService.GetCartAsync();
                        var cart = cartResponse.Data;
                        var purchasedMovies = cart.CartItems.Select(item => new MovieInfo
                        {
                            MovieId = item.MovieId,
                            MovieTitle = item.MovieTitle,
                            MoviePrice = item.Price
                        }).ToList();



                        var userEmail = _httpContextAccessor.HttpContext.User.Claims
                            .FirstOrDefault(c => c.Type == "email" || c.Type == "emails")?.Value;

                        if (!string.IsNullOrEmpty(userEmail))
                        {
                            var subject = "Your Purchase Receipt";
                            var htmlContent = GenerateHtmlInvoice(cart, verificationResponse.data);
                            var plainTextContent = GeneratePlainTextInvoice(cart, verificationResponse.data);

                            var emailSent = await _emailService.SendEmailAsync(userEmail, subject, htmlContent, plainTextContent);

                            if (emailSent)
                            {
                                _logger.LogInformation($"Invoice sent to {userEmail} for reference {reference}.");
                            }
                            else
                            {
                                _logger.LogError($"Failed to send invoice to {userEmail} for reference {reference}.");
                            }
                        }
                        else
                        {
                            _logger.LogWarning($"User email not found for reference {reference}. Invoice not sent.");
                        }

                        return new PaymentResult
                        {
                            Success = true,
                            TransactionId = verificationResponse.data.reference,
                            Message = "Payment verified successfully.",
                            AmountPaid = verificationResponse.data.amount / 100m,
                            PurchasedMovies = purchasedMovies,
                            PayerEmail = verificationResponse.data.payerEmail


                        };
                    }
                    else
                    {
                        _logger.LogError($"Payment verification failed for reference: {reference}");
                        return new PaymentResult { Success = false, Message = "Payment verification failed." };
                    }
                }
                else
                {
                    _logger.LogError($"Failed to verify payment. Reference: {reference}, Response: {responseContent}");
                    return new PaymentResult { Success = false, Message = "Failed to verify payment with Paystack." };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying Paystack payment.");
                return new PaymentResult { Success = false, Message = "An error occurred while verifying the Paystack payment." };
            }
        }


        private string GenerateHtmlInvoice(Cart cart, PaystackData data)
        {
            var sb = new StringBuilder();
            sb.Append("<h1>Thank you for your purchase!</h1>");
            sb.Append("<h2>Order Details:</h2>");
            sb.Append("<ul>");
            foreach (var item in cart.CartItems)
            {
                sb.Append($"<li>{item.MovieTitle} - Quantity: {item.Quantity} - Price: ₦{item.Price}</li>");
            }
            sb.Append("</ul>");
            sb.Append($"<p>Total Amount Paid: ₦{data.amount / 100m}</p>");
            sb.Append($"<p>Transaction Reference: {data.reference}</p>");
            sb.Append("<p>We hope you enjoy your movies!</p>");
            return sb.ToString();
        }

        private string GeneratePlainTextInvoice(Cart cart, PaystackData data)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Thank you for your purchase!");
            sb.AppendLine("Order Details:");
            foreach (var item in cart.CartItems)
            {
                sb.AppendLine($"- {item.MovieTitle} - Quantity: {item.Quantity} - Price: ₦{item.Price}");
            }
            sb.AppendLine($"Total Amount Paid: ₦{data.amount / 100m}");
            sb.AppendLine($"Transaction Reference: {data.reference}");
            sb.AppendLine("We hope you enjoy your movies!");
            return sb.ToString();
        }
    }
}
