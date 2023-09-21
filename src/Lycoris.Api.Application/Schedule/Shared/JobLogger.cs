using Lycoris.Api.Core.Logging;

namespace Lycoris.Api.Application.Schedule.Shared
{
    /// <summary>
    /// 
    /// </summary>
    public class JobLogger
    {
        private readonly ILycorisLogger _logger;

        /// <summary>
        /// 
        /// </summary>
        private string? JobTraceId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private string? JobName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        public JobLogger(ILycorisLogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="JobTraceId"></param>
        /// <param name="JobName"></param>
        internal void JobWorkRegister(string? JobTraceId, string? JobName)
        {
            this.JobTraceId = JobTraceId ?? "";
            this.JobName = JobName ?? "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        internal void Info(string message) => this._logger?.Info(ChangeMessage(message), this.JobTraceId!);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        internal void Warn(string message, Exception? exception = null) => this._logger?.Warn(ChangeMessage(message), exception, this.JobTraceId!);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        internal void Error(string message, Exception? exception = null) => this._logger?.Error(ChangeMessage(message), exception, this.JobTraceId!);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private string ChangeMessage(string message) => $"{this.JobName} - {message}";
    }
}
