using InternshipTradingApp.AccountManagement.DTOs;
using InternshipTradingApp.AccountManagement.Entities;
using InternshipTradingApp.AccountManagement.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;

namespace InternshipTradingApp.Server.Controllers.AccountManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class StripeController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IFundsService _fundsService;

        public StripeController(UserManager<AppUser> userManager, IFundsService fundsService)
        {
            _userManager = userManager;
            _fundsService = fundsService;
        }

        [HttpPost("create-checkout-session")]
        public async Task<IActionResult> CreateCheckoutSession([FromBody] AddFundsDto request)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = request.Amount * 100,
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Deposit funds"
                            },
                        },
                        Quantity = 1,
                    },
                },
                Mode = "payment",
                SuccessUrl = $"http://localhost:4200/success?amount={request.Amount}&userId={user.Id}&username={Uri.EscapeDataString(user.UserName!)}",
                CancelUrl = "http://localhost:4200/cancel",
                Metadata = new Dictionary<string, string>
                {
                    { "UserId", user.Id.ToString() },
                    { "Username", user.UserName! }
                }
            };

            var service = new SessionService();
            Session session = await service.CreateAsync(options);

            return Ok(new { url = session.Url });
        }



        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebhook([FromBody] object payload)
        {
            var json = await new StreamReader(Request.Body).ReadToEndAsync();
            var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], "whsec_bc9ca4f1726b53a4a44d353e1242454844f4cb33c170018b9ebd0f9e24b1c0a3");

            switch (stripeEvent.Type)
            {
                case Events.CheckoutSessionCompleted:
                    var session = stripeEvent.Data.Object as Session;

                    var userId = session.Metadata["UserId"];
                    var username = session.Metadata["Username"];

                    var user = await _userManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        Console.WriteLine("dsadsa");
                    }
                    break;

                default:
                    Console.WriteLine($"Unhandled event type {stripeEvent.Type}");
                    break;
            }

            return Ok();
        }


    }
}
