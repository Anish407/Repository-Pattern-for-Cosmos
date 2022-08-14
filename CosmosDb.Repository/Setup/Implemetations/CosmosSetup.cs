using Azure.Identity;
using CosmosDb.Repository.Setup.interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CosmosDb.Repository.Setup.Implemetations
{
    public class CosmosSetup : ICosmosSetup
    {
        public CosmosSetup(
            IConfiguration configuration,
            ILoggerFactory loggerFactory,
            IHttpClientFactory httpClientFactory)
        {
            Logger = loggerFactory.CreateLogger<CosmosSetup>();
            HttpClient = httpClientFactory.CreateClient(nameof(CosmosSetup));
            Configuration = configuration;
        }

        private HttpClient HttpClient { get; }

        public IConfiguration Configuration { get; }

        public ILogger Logger { get; }

        /// <summary>
        ///  Setup cosmos client and return the instance. This instance needs to be registered as singleton
        /// </summary>
        /// <param name="cotainersList"> List of containers and collections to initialize
        ///  List<(string, string)>? cotainersList = new List<(string, string)>
        ///     {
        ///      ("ContainerName","CollectionName"),
        ///     };
        /// </param>
        /// <returns></returns>
        public async Task<CosmosClient> SetupCosmosClient(List<(string, string)> cotainersList)
        {
            // Add null checks (write an extension or guard class)
            // Connect using RBAC, Either create a custom role or assign the "Cosmos DB Built-in Data Contributor" role
            // https://docs.microsoft.com/en-us/azure/cosmos-db/how-to-setup-rbac
            //  https://github.com/Anish407/Azure.Identity.Samples/tree/master/Azure.Identity.Samples/CosmosDb
            return await CosmosClient.CreateAndInitializeAsync(Configuration["CosmosEndPointUrl"], new DefaultAzureCredential(), cotainersList);

        }
    }
}
