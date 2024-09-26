namespace Application.Service.Abstraction
{
    public interface ICosmosDbService<T> where T : class
    {
        Task AddItemAsync(T item, string partitionKey);
        Task AddItemAsyncz(T item);
        Task DeleteItemAsync(string id, string partitionKey);
        Task<T> GetItemAsync(string id, string partitionKey);
        Task<T> GetItemAsyncz(string id);
        Task<IEnumerable<T>> GetItemsAsync(string query);
        Task<(IEnumerable<T> Items, string ContinuationToken)> GetItemsWithContinuationTokenAsync(string continuationToken, int maxItemCount = 30, string partitionKey = null);
        Task UpdateItemAsync(string id, T item, string partitionKey);
    }
}