using CosmosDb.Repository.Common.Helpers;
using CosmosDb.Repository.Models;
using CosmosDb.Repository.Repository.Interfaces;
using Microsoft.Azure.Cosmos;

namespace CosmosDb.Repository.Repository.Implementations
{
    public abstract class CosmosRepository<TItem> : IRepository<TItem>
         where TItem : IDocument
    {
        public abstract string DatabaseId { get; }

        public abstract string ContainerId { get; }

        private TryCatchWrapper _wrapper = new TryCatchWrapper();

        protected Container Container { get; set; }

        public CosmosRepository(CosmosClient cosmosClient)
        {
            CosmosClient = cosmosClient;
            Container = CosmosClient.GetContainer(DatabaseId, ContainerId);
        }

        public CosmosClient CosmosClient { get; }

        public async ValueTask<TItem> GetItem(string id, string partitionKeyValue = "", CancellationToken cancellationToken = default)
        => await GetItem(id, new PartitionKey(partitionKeyValue ?? id), cancellationToken);

        public async ValueTask<TItem> GetItem(string id, PartitionKey partitionKey, CancellationToken cancellationToken = default)
        {
            return await _wrapper.TryCatch(
              async () =>
              {
                  if (partitionKey == default)
                  {
                      partitionKey = new PartitionKey(id);
                  }

                  ItemResponse<TItem> response =
                      await Container.ReadItemAsync<TItem>(id, partitionKey, cancellationToken: cancellationToken);

                  TItem item = response.Resource;
                  item.ETag = response.ETag;

                  return item;
              }, string.Format("Id:{0} and partitionKey:{1}", id, partitionKey));
        }

        public async ValueTask<IEnumerable<TItem>> GetBySQLQuery(
            string query,
            CancellationToken cancellationToken = default)
        {
            return await _wrapper.TryCatch(async () =>
            {
                QueryDefinition queryDefinition = new(query);
                return await ReadItems(queryDefinition, cancellationToken);
            });
        }

        public async ValueTask<TItem> Create(
            TItem value,
            CancellationToken cancellationToken = default)
        {
            return await _wrapper.TryCatch(async () =>
            {
                ItemResponse<TItem> response =
                 await Container.CreateItemAsync(value, new PartitionKey(value.PartitionKey), cancellationToken: cancellationToken);

                return response.Resource;
            });
        }

        private async Task<IEnumerable<TItem>> ReadItems(QueryDefinition queryDefinition, CancellationToken cancellationToken)
        {
            using FeedIterator<TItem> queryResultSetIterator = Container.GetItemQueryIterator<TItem>(queryDefinition);

            List<TItem> results = new();

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<TItem> response = await queryResultSetIterator.ReadNextAsync(cancellationToken).ConfigureAwait(false);
                results.AddRange(response.Resource);
            }

            return results;
        }

        public async ValueTask<TItem> Update(
            TItem value,
            CancellationToken cancellationToken = default)
        {
            return await _wrapper.TryCatch(async () =>
            {
                ItemResponse<TItem> response =
                await Container.UpsertItemAsync(value, new PartitionKey(value.PartitionKey), cancellationToken: cancellationToken);

                return response.Resource;
            });
        }

        public async ValueTask<bool> Delete(
            string id,
            PartitionKey partitionKey = default,
            CancellationToken cancellationToken = default)
        {
            return await _wrapper.TryCatch<bool>(async () =>
            {
                if (partitionKey == default)
                {
                    partitionKey = new PartitionKey(id);
                }

                _ = await Container.DeleteItemAsync<TItem>(id, partitionKey, cancellationToken: cancellationToken);
                return true;
            });
        }

        public async ValueTask<IEnumerable<TItem>> CreateItems(
           IEnumerable<TItem> values,
           CancellationToken cancellationToken = default)
        {
            IEnumerable<Task<TItem>> creationTasks =
                values.Select(value => CreateAsync(value, cancellationToken).AsTask())
                    .ToList();

            _ = await Task.WhenAll(creationTasks).ConfigureAwait(false);

            return creationTasks.Select(x => x.Result);
        }

        public async Task<TransactionalBatchResponse> ExecuteTransaction(TransactionalBatch batch)
        {
            return await batch.ExecuteAsync();
        }
    }
}
