using System.Threading.Tasks;

namespace EggPlantApi.Integration
{
    public interface IIntegrationEventHandler<in T> where T : IntegrationEvent
    {
        Task<object> Handle(T eventData);
    }

    public interface IIntegrationEventHandler
    {
    }
}
