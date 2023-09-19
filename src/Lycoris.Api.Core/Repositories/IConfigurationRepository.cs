namespace Lycoris.Api.Core.Repositories
{
    public interface IConfigurationRepository
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configId"></param>
        /// <returns></returns>
        Task<string> GetConfigurationAsync(string configId);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configId"></param>
        /// <returns></returns>
        Task<T?> GetConfigurationAsync<T>(string configId) where T : class;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configId"></param>
        /// <returns></returns>
        Task RemoveConfigurationCacheAsync(string configId);
    }
}
