using System;

namespace EggOrdersApi.Integration
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