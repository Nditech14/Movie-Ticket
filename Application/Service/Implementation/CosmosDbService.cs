using Application.Service.Abstraction;
using Core.Abstraction;

namespace Core
{
    public class CosmosDbService<T> : ICosmosDbService<T> where T : class
    {
        private readonly ICosmosDbRepository<T> _repository;

        public CosmosDbService(ICosmosDbRepository<T> repository)
        {
            _repository = repository;
        }


        public async Task<T> GetItemAsync(string id, string partitionKey)
        {
            return await _repository.GetItemAsync(id, partitionKey);
        }


        public async Task<IEnumerable<T>> GetItemsAsync(string query)
        {
            return await _repository.GetItemsAsync(query);
        }

        public async Task<T> GetItemAsyncz(string id)
        {
            return await _repository.GetItemAsyncz(id);
        }


        public async Task AddItemAsync(T item, string partitionKey)
        {
            await _repository.AddItemAsync(item, partitionKey);
        }


        public async Task UpdateItemAsync(string id, T item, string partitionKey)
        {
            await _repository.UpdateItemAsync(id, item, partitionKey);
        }


        public async Task DeleteItemAsync(string id, string partitionKey)
        {
            await _repository.DeleteItemAsync(id, partitionKey);
        }


        public async Task<(IEnumerable<T> Items, string ContinuationToken)> GetItemsWithContinuationTokenAsync(string continuationToken, int maxItemCount = 30, string partitionKey = null)
        {
            return await _repository.GetItemsWithContinuationTokenAsync(continuationToken, maxItemCount, partitionKey);
        }

        public async Task AddItemAsyncz(T item)
        {
            await _repository.AddItemAsyncz(item);
        }
    }
}
