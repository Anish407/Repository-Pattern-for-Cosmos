namespace CosmosDb.Repository.Models
{
    public abstract class IDocument
    {
       public  string Id { get; set; }

        public string PartitionKey { get; }

        public string ETag { get; set; }
    }
}
