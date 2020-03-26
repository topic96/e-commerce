using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Shop.Database;
using Shop.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shop.Application.Cart
{
    public class GetCart
    {
        private ISession _session;
        private ApplicationDbContext _ctx;

        public GetCart(ISession session, ApplicationDbContext ctx)
        {
            _session = session;
            _ctx = ctx;
        }

        public class Response
        {
            public string Name { get; set; }
            public string Value { get; set; }
            public int Qty { get; set; }

            public int StockId { get; set; }
        }

        public IEnumerable<Response> Do()
        {
            //TODO: account for multiple items in the cart
            var stringObject = _session.GetString("cart");

            if (string.IsNullOrEmpty(stringObject))
                return new List<Response>();

            var cartList = JsonConvert.DeserializeObject<List<CartProduct>>(stringObject);

            var itemsInCart = cartList.Select(x => x.StockId).ToList();

            var response = _ctx.Stock
                .Include(x => x.Product)
                .Where(x => itemsInCart.Contains(x.Id))
                .Select(x => new Response
                {
                    Name = x.Product.Name,
                    Value = $"RSD {x.Product.Value:N2}",
                    StockId = x.Id

                })
                .ToList();

            response = response.Select(x =>
            {
                x.Qty = cartList.FirstOrDefault(y => y.StockId == x.StockId).Qty;
                return x;
            })
            .ToList();

            return response;
        }
    }
}
