using CosmosDb.Repository.Models;
using CosmosDb.Repository.Repository.Interfaces;
using Microsoft.Azure.Cosmos;

namespace CosmosDb.Repository.Repository.Implementations
{
    public class StudentRepository : CosmosRepository<Student>, IStudentRepository
    {
        public StudentRepository(CosmosClient cosmosClient)
            : base(cosmosClient)
        {
        }

        public override string DatabaseId => "DemoContainer";

        public override string ContainerId => "Data";
    }
}
