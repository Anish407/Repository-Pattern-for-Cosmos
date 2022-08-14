using Microsoft.Azure.Cosmos;

namespace CosmosDb.Repository.Setup.interfaces
{
    public interface ICosmosSetup
    {
        Task<CosmosClient> SetupCosmosClient(List<(string, string)> cotainersList);
    }
}
