namespace Infrastructure.CosmosFig
{
    public class CosmosDbSettings
    {
        public string Account { get; set; }
        public string Key { get; set; }
        public string DatabaseName { get; set; }
        public CosmosDbContainers Containers { get; set; }
    }
}
