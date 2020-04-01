using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Shop.Application.Cart;
using Shop.Application.Orders;
using Shop.Database;
using Stripe;

namespace Shop.UI.Pages.Checkout
{
    public class PaymentModel : PageModel
    {
        private ApplicationDbContext _ctx;

        public string PublicKey { get;}

        public PaymentModel(IConfiguration config, ApplicationDbContext ctx)
        {
            _ctx = ctx;

            PublicKey = config["Stripe:PublicKey"].ToString();
        }

        //public string SecretKey { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var information = new GetCustomerInformation(HttpContext.Session).Do();
            if (information == null)
                return RedirectToPage("/Checkout/CustomerInformation");


            var CartOrder = new Application.Cart.GetOrder(HttpContext.Session, _ctx).Do();

            // STRIPE -------------------------------------------------------
            var service = new PaymentIntentService();

            var options = new PaymentIntentCreateOptions
            {
                Amount = CartOrder.GetTotalCharge(),
                Description = "Shop Purchase",
                Currency = "rsd",
                // Verify your integration in this guide by including this parameter
                Metadata = new Dictionary<String, String>()
                {
                  {"integration_check", "accept_a_payment"},
                }
            };

            var paymentIntent = service.Create(options);

            ViewData["ClientSecret"] = paymentIntent.ClientSecret;
            // --------------------------------------------------------------

            var sessionId = HttpContext.Session.Id;

            await new CreateOrder(_ctx).Do(new CreateOrder.Request
            {
                //order id - try out
                StripeReference = paymentIntent.Id,
                SessionId = sessionId,

                FirstName = CartOrder.CustomerInformation.FirstName,
                LastName = CartOrder.CustomerInformation.LastName,
                Email = CartOrder.CustomerInformation.Email,
                PhoneNumber = CartOrder.CustomerInformation.PhoneNumber,
                Address1 = CartOrder.CustomerInformation.Address1,
                Address2 = CartOrder.CustomerInformation.Address2,
                City = CartOrder.CustomerInformation.City,
                PostCode = CartOrder.CustomerInformation.PostCode,

                Stocks = CartOrder.Products.Select(x => new CreateOrder.Stock
                {
                    StockId = x.StockId,
                    Qty = x.Qty
                })
                .ToList()
            });


            return RedirectToPage("/Index");

        }

        public IActionResult OnPost()
        {


            return RedirectToPage("/Index");
        }


    }
}
