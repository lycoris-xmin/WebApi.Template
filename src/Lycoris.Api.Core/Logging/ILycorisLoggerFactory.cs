namespace Lycoris.Api.Core.Logging
{
    /// <summary>
    /// 日志工厂
    /// </summary>
    public interface ILycorisLoggerFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        ILycorisLogger CreateLogger<T>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        ILycorisLogger CreateLogger(Type type);
    }
}
