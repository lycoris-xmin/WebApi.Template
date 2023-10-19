using Lycoris.Api.Model.Cnstants;
using Lycoris.Common.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Lycoris.Api.Core.Logging.Impl
{
    public class LycorisLogger : ILycorisLogger
    {
        private readonly ILogger _logger;
        private readonly IHttpContextAccessor _context;

        public LycorisLogger(ILogger logger, IHttpContextAccessor context)
        {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// 日志记录
        /// </summary>
        /// <param name="message"></param>
        public void Info(string message)
        {
            var traceId = GetLogTraceId();
            if (!traceId.IsNullOrEmpty())
                _logger.LogInformation("{traceId} - {message}", GetLogTraceId(traceId), message);
            else
                _logger.LogInformation("{message}", message);
        }

        /// <summary>
        /// 日志记录
        /// </summary>
        /// <param name="message"></param>
        /// <param name="traceId"></param>
        public void Info(string message, string traceId)
        {
            traceId = GetLogTraceId(traceId);
            if (!traceId.IsNullOrEmpty())
                _logger.LogInformation("{traceId} - {message}", GetLogTraceId(traceId), message);
            else
                _logger.LogInformation("{message}", message);
        }

        /// <summary>
        /// 日志记录
        /// </summary>
        /// <param name="message"></param>
        public void Warn(string message)
        {
            var traceId = GetLogTraceId();
            if (!traceId.IsNullOrEmpty())
                _logger.LogWarning("{traceId} - {message}", traceId, message);
            else
                _logger.LogWarning("{message}", message);
        }

        /// <summary>
        /// 日志记录
        /// </summary>
        /// <param name="message"></param>
        /// <param name="traceId"></param>
        public void Warn(string message, string traceId)
        {
            traceId = GetLogTraceId(traceId);
            if (!traceId.IsNullOrEmpty())
                _logger.LogWarning("{traceId} - {message}", traceId, message);
            else
                _logger.LogWarning("{message}", message);
        }

        /// <summary>
        /// 日志记录
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public void Warn(string message, Exception? ex)
        {
            var traceId = GetLogTraceId();
            if (!traceId.IsNullOrEmpty())
                _logger.LogWarning(ex, "{traceId} - {message}", traceId, message);
            else
                _logger.LogWarning(ex, "{message}", message);
        }

        /// <summary>
        /// 日志记录
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        /// <param name="traceId"></param>
        public void Warn(string message, Exception? ex, string traceId)
        {
            traceId = GetLogTraceId(traceId);
            if (!traceId.IsNullOrEmpty())
                _logger.LogWarning(ex, "{traceId} - {message}", traceId, message);
            else
                _logger.LogWarning(ex, "{message}", message);
        }

        /// <summary>
        /// 日志记录
        /// </summary>
        /// <param name="message"></param>
        public void Error(string message)
        {
            var traceId = GetLogTraceId();
            if (!traceId.IsNullOrEmpty())
                _logger.LogError("{traceId} - {message}", traceId, message);
            else
                _logger.LogError("{message}", message);
        }

        /// <summary>
        /// 日志记录
        /// </summary>
        /// <param name="message"></param>
        /// <param name="traceId"></param>
        public void Error(string message, string traceId)
        {
            traceId = GetLogTraceId(traceId);
            if (!traceId.IsNullOrEmpty())
                _logger.LogError("{traceId} - {message}", traceId, message);
            else
                _logger.LogError("{message}", message);
        }

        /// <summary>
        /// 日志记录
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public void Error(string message, Exception? ex)
        {
            var traceId = GetLogTraceId();
            if (!traceId.IsNullOrEmpty())
                _logger.LogError(ex, "{traceId} - {message}", traceId, message);
            else
                _logger.LogError(ex, "{message}", message);
        }

        /// <summary>
        /// 日志记录
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        /// <param name="traceId"></param>
        public void Error(string message, Exception? ex, string traceId)
        {
            traceId = GetLogTraceId(traceId);
            if (!traceId.IsNullOrEmpty())
                _logger.LogError(ex, "{traceId} - {message}", traceId, message);
            else
                _logger.LogError(ex, "{message}", message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="traceId"></param>
        /// <returns></returns>
        private string GetLogTraceId(string? traceId = null)
        {
            if (!traceId.IsNullOrEmpty())
                return traceId!;

            if (_context != null && _context.HttpContext != null && _context.HttpContext.Items != null)
                traceId = _context.HttpContext.Items.GetValue(HttpItems.TraceId);

            if (!traceId.IsNullOrEmpty())
                return traceId!;

            return "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logLevel"></param>
        /// <returns></returns>
        public bool IsEnabled(LogLevel logLevel) => _logger.IsEnabled(logLevel);
    }
}
