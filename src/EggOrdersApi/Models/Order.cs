using System;

namespace EggOrdersApi.Models
{
    public class Order
    {
        public string OrderId { get; set; }
        public string ClientName { get; set; }
        public string ClientSurname { get; set; }
        public int Quantity { get; set; }
        public string Status { get; set; }

        public Order()
        {
            if (string.IsNullOrEmpty(OrderId))
            {
                OrderId = Guid.NewGuid().ToString();
            }

            if (string.IsNullOrEmpty(Status))
            {
                Status = "Created";
            }
        }
    }
}
