using System;

namespace EggPlantApi.Integration
{
    public class IntegrationEvent
    {
        public IntegrationEvent()
        {
            Guid = Guid.NewGuid();
            CreationDate = DateTime.Now;
        }

        public Guid Guid { get; }
        public DateTime CreationDate { get; }
    }
}