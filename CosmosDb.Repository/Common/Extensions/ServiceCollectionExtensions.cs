using CosmosDb.Repository.Repository.Implementations;
using CosmosDb.Repository.Repository.Interfaces;
using CosmosDb.Repository.Setup.Implemetations;
using CosmosDb.Repository.Setup.interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CosmosDb.Repository.Common.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterCosmosDb(this IServiceCollection services)
        {
            return services
                .AddScoped<IStudentRepository, StudentRepository>()
                .AddScoped<ICosmosSetup, CosmosSetup>();
        }
    }
}
