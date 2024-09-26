using Core.Abstraction;
using Core.Entities;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

namespace Core.Implementation
{
    public class CosmosDbRepository<T> : ICosmosDbRepository<T> where T : class
    {
        private readonly Container _MovieContainer;
        private readonly Container _ActorContainer;
        private readonly Container _CinemaContainer;
        private readonly Container _ProducerContainer;
        private readonly Container _TicketPaymentContainer;
        private readonly Container _CartDetailsContainer;

        public CosmosDbRepository(CosmosClient dbClient, IConfiguration configuration)
        {
            var databaseName = configuration["CosmosDb:DatabaseName"];
            _MovieContainer = dbClient.GetContainer(databaseName, configuration["CosmosDb:Containers:MovieContainer"]);
            _ActorContainer = dbClient.GetContainer(databaseName, configuration["CosmosDb:Containers:ActorContainer"]);
            _CinemaContainer = dbClient.GetContainer(databaseName, configuration["CosmosDb:Containers:CinemaContainer"]);
            _ProducerContainer = dbClient.GetContainer(databaseName, configuration["CosmosDb:Containers:ProducerContainer"]);
            _CartDetailsContainer = dbClient.GetContainer(databaseName, configuration["CosmosDb:Containers:CartDetailsContainer"]);
        }

        // Dynamically gets the correct container based on type T
        private Container GetContainer()
        {
            if (typeof(T) == typeof(Movie))
            {
                return _MovieContainer;
            }
            else if (typeof(T) == typeof(Actor))
            {
                return _ActorContainer;
            }
            else if (typeof(T) == typeof(Cinema))
            {
                return _CinemaContainer;
            }
            else if (typeof(T) == typeof(Producer))
            {
                return _ProducerContainer;
            }
            else if (typeof(T) == typeof(Cart))
            {
                return _CartDetailsContainer;
            }
            else
            {
                throw new ArgumentException($"No container available for type {typeof(T).Name}");
            }
        }


        public async Task<T> GetItemAsync(string id, string partitionKey)
        {
            try
            {
                var _container = GetContainer();
                ItemResponse<T> response = await _container.ReadItemAsync<T>(id, new PartitionKey(partitionKey));
                return response.Resource;
            }
            catch (CosmosException)
            {
                return null;
            }
        }


        public async Task<IEnumerable<T>> GetItemsAsync(string queryString)
        {
            var _container = GetContainer();
            var query = _container.GetItemQueryIterator<T>(new QueryDefinition(queryString));
            List<T> results = new List<T>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }
            return results;
        }
        public async Task AddItemAsyncz(T item)
        {
            var _container = GetContainer();
            await _container.CreateItemAsync(item);

        }

        public async Task<T> GetItemAsyncz(string id)
        {
            var _container = GetContainer();


            var queryDefinition = new QueryDefinition("SELECT * FROM c WHERE c.id = @id")
                                    .WithParameter("@id", id);


            var queryIterator = _container.GetItemQueryIterator<T>(queryDefinition);


            if (queryIterator.HasMoreResults)
            {
                var response = await queryIterator.ReadNextAsync();
                return response.FirstOrDefault();
            }

            return null;
        }

        public async Task AddItemAsync(T item, string partitionKey)
        {
            var _container = GetContainer();
            await _container.CreateItemAsync(item, new PartitionKey(partitionKey));
        }


        public async Task UpdateItemAsync(string id, T item, string partitionKey)
        {
            var _container = GetContainer();
            await _container.UpsertItemAsync(item, new PartitionKey(partitionKey));
        }


        public async Task DeleteItemAsync(string id, string partitionKey)
        {
            var _container = GetContainer();
            await _container.DeleteItemAsync<T>(id, new PartitionKey(partitionKey));
        }


        public async Task<(IEnumerable<T> Items, string ContinuationToken)> GetItemsWithContinuationTokenAsync(string continuationToken, int maxItemCount = 30, string partitionKey = null)
        {
            var _container = GetContainer();
            var queryRequestOptions = new QueryRequestOptions { MaxItemCount = maxItemCount };

            if (partitionKey != null)
            {
                queryRequestOptions.PartitionKey = new PartitionKey(partitionKey);
            }

            var queryIterator = _container.GetItemQueryIterator<T>(continuationToken: continuationToken, requestOptions: queryRequestOptions);

            var results = new List<T>();
            string newContinuationToken = null;

            while (queryIterator.HasMoreResults)
            {
                var response = await queryIterator.ReadNextAsync();
                results.AddRange(response);
                newContinuationToken = response.ContinuationToken;

                if (results.Count >= maxItemCount)
                {
                    break;
                }
            }

            return (results, newContinuationToken);
        }


        public FeedIterator<T> GetItemQueryIterator(QueryDefinition query, string continuationToken = null, QueryRequestOptions requestOptions = null)
        {
            var _container = GetContainer();
            return _container.GetItemQueryIterator<T>(query, continuationToken, requestOptions);
        }
    }
}
