using EggOrdersApi.Integration;

namespace EggOrdersApi.Events
{
    public class NewOrderCreatedIntegrationEvent : IntegrationEvent
    {
        public string OrderId { get; set; }
        public string ClientId { get; set; }
        public int Quantity { get; set; }
    }
}
