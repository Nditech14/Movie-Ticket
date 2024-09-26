using Microsoft.Azure.Cosmos;

namespace Core.Abstraction
{
    public interface ICosmosDbRepository<T> where T : class
    {
        Task AddItemAsync(T item, string partitionKey);
        Task AddItemAsyncz(T item);
        Task DeleteItemAsync(string id, string partitionKey);
        Task<T> GetItemAsync(string id, string partitionKey);
        Task<T> GetItemAsyncz(string id);
        FeedIterator<T> GetItemQueryIterator(QueryDefinition query, string continuationToken = null, QueryRequestOptions requestOptions = null);
        Task<IEnumerable<T>> GetItemsAsync(string queryString);
        Task<(IEnumerable<T> Items, string ContinuationToken)> GetItemsWithContinuationTokenAsync(string continuationToken, int maxItemCount = 30, string partitionKey = null);
        Task UpdateItemAsync(string id, T item, string partitionKey);
    }
}