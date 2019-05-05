using System;
using System.Collections.Generic;

namespace EggPlantApi.Integration
{
    internal class SubscribedEvents
    {
        public Type EventType { get; set; }
        public List<Type> Handlers { get; set; }

        public SubscribedEvents()
        {
            Handlers = new List<Type>();
        }
    }
}