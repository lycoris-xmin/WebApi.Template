using Microsoft.Extensions.Logging;

namespace Lycoris.Api.Core.Logging
{
    /// <summary>
    /// 
    /// </summary>
    public interface ILycorisLogger
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logLevel"></param>
        /// <returns></returns>
        bool IsEnabled(LogLevel logLevel);

        /// <summary>
        /// 日志记录
        /// </summary>
        /// <param name="message">日志内容</param>
        void Info(string message);

        /// <summary>
        /// 日志记录
        /// </summary>
        /// <param name="message">日志内容</param>
        /// <param name="traceId"></param>
        void Info(string message, string traceId);

        /// <summary>
        /// 日志记录
        /// </summary>
        /// <param name="message">日志内容</param>
        void Warn(string message);

        /// <summary>
        /// 日志记录
        /// </summary>
        /// <param name="message">日志内容</param>
        /// <param name="traceId"></param>
        void Warn(string message, string traceId);

        /// <summary>
        /// 日志记录
        /// </summary>
        /// <param name="message">日志内容</param>
        /// <param name="ex"><see cref="Exception"/>异常信息</param>
        void Warn(string message, Exception? ex);

        /// <summary>
        /// 日志记录
        /// </summary>
        /// <param name="message">日志内容</param>
        /// <param name="ex"><see cref="Exception"/>异常信息</param>
        /// <param name="traceId"></param>
        void Warn(string message, Exception? ex, string traceId);

        /// <summary>
        /// 日志记录
        /// </summary>
        /// <param name="message">日志内容</param>
        void Error(string message);

        /// <summary>
        /// 日志记录
        /// </summary>
        /// <param name="message">日志内容</param>
        /// <param name="traceId"></param>
        void Error(string message, string traceId);

        /// <summary>
        /// 日志记录
        /// </summary>
        /// <param name="message">日志内容</param>
        /// <param name="ex"><see cref="Exception"/></param>
        void Error(string message, Exception? ex);

        /// <summary>
        /// 日志记录
        /// </summary>
        /// <param name="message">日志内容</param>
        /// <param name="ex"><see cref="Exception"/>异常信息</param>
        /// <param name="traceId"></param>
        void Error(string message, Exception? ex, string traceId);
    }
}
