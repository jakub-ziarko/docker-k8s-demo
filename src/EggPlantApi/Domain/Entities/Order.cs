using System.Collections.Generic;

namespace EggPlantApi.Domain.Entities
{
    public class Order
    {
        public string ClientId { get; set; }
        public IList<Egg> Eggs { get; set; }
    }
}
