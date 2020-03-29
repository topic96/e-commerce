using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Shop.Application.Cart;
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

        public IActionResult OnGet()
        {
            var information = new GetCustomerInformation(HttpContext.Session).Do();
            if (information == null)
                return RedirectToPage("/Checkout/CustomerInformation");


            var CartOrder = new GetOrder(HttpContext.Session, _ctx).Do();

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


            return RedirectToPage("/Index");

        }

        public IActionResult OnPost()
        {


            return RedirectToPage("/Index");
        }


    }
}
