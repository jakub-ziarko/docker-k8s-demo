using System;

namespace EggPlantApi.Domain.Entities
{
    public class Egg
    {
        public string Id { get; set; }
        public DateTime Created { get; set; }
        public string Name { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }
        public double Radius { get; set; }

        public Egg()
        {
            Id = Guid.NewGuid().ToString();
            Created = DateTime.Now;
            Name = $"Egg - {Id.Substring(0, 8)}";
        }
    }
}
