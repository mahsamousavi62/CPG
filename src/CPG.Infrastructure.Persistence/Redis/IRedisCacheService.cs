namespace CPG.Infrastructure.Persistence.Redis
{
    public interface IRedisCacheService
    {
        T GetData<T>(string key);
        void SetData<T>(string key, T value);
        void RemoveKey(string key);
    }
}
