using Lycoris.Api.Core.Logging;

namespace Lycoris.Api.Application.RqbbitMq.Impl
{
    public class RabbitLogger
    {
        private readonly ILycorisLogger Logger;
        private readonly string ConsumerName;
        private readonly string Exchange;
        private readonly string Route;
        private readonly string TraceId;


        public RabbitLogger(ILycorisLogger Logger, string ConsumerName, string Exchange, string Route, string TraceId)
        {
            this.Logger = Logger;
            this.ConsumerName = ConsumerName;
            this.Exchange = Exchange;
            this.Route = Route;
            this.TraceId = TraceId;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void Info(string message) => Logger.Info($"{ConsumerName} - rabbitmq({Exchange}/{Route}) -> {message}", TraceId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void Warn(string message) => Logger.Warn($"{ConsumerName} - rabbitmq({Exchange}/{Route}) -> {message}", TraceId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void Warn(string message, Exception ex) => Logger.Warn($"{ConsumerName} - rabbitmq({Exchange}/{Route}) -> {message}", ex, TraceId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void Error(string message) => Logger.Error($"{ConsumerName} - rabbitmq({Exchange}/{Route}) -> {message}", TraceId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void Error(string message, Exception ex) => Logger.Error($"{ConsumerName} - rabbitmq({Exchange}/{Route}) -> {message}", ex, TraceId);
    }
}
