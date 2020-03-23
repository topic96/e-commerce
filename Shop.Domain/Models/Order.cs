using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Domain.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string OrderRef { get; set; }

        public string Adress1 { get; set; }
        public string Adress2 { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }      
        
        public ICollection<OrderProduct> OrderProducts { get; set; }
    }
}
