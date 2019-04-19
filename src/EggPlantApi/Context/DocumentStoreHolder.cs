using Microsoft.Extensions.Options;
using Raven.Client.Documents;

namespace EggPlantApi.Context
{
    public class DocumentStoreHolder
    {
        public DocumentStoreHolder(IOptions<RavenSettings> ravenSettings)
        {
            var settings = ravenSettings.Value;

            Store = new DocumentStore
            {
                Urls = settings.Url,
                Database = settings.DefaultDatabase
            }.Initialize();
        }

        public IDocumentStore Store { get; }
    }
}
