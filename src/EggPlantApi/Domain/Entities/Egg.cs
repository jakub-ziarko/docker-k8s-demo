using System;

namespace EggPlantApi.Domain.Entities
{
    public class Egg
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }
        public double Radius { get; set; }

        public Egg FillGuid()
        {
            Id = Guid.NewGuid();
            return this;
        }
    }
}
