using EggPlantApi.Integration;

namespace EggPlantApi.Domain.Events
{
    public class NewOrderCreatedIntegrationEvent : IntegrationEvent
    {
        public string ClientId { get; set; }
        public int Quantity { get; set; }
    }
}
