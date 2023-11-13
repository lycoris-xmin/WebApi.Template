using Lycoris.Api.Core.Logging;
using Lycoris.Common.Extensions;
using Lycoris.RabbitMQ.Extensions;
using Lycoris.RabbitMQ.Extensions.DataModel;
using System.Diagnostics;

namespace Lycoris.Api.Application.RqbbitMq.Impl
{
    public abstract class RabbitConsumerListener : BaseRabbitConsumerListener
    {
        protected string ConsumerName { get; private set; }

        protected string TraceId { get; private set; }

        protected RabbitLogger RabbitLogger { get; private set; }

        private readonly ILycorisLogger _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="ConsumerName"></param>
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        public RabbitConsumerListener(ILycorisLogger logger, string ConsumerName)
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        {
            _logger = logger;
            TraceId = Guid.NewGuid().ToString("N");
            this.ConsumerName = ConsumerName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        protected override async Task<ReceivedHandler> ReceivedAsync(string body)
        {
            var watch = new Stopwatch();
            watch.Start();

            if (!body.IsNullOrEmpty())
            {
                var jObject = body.ToJObject();
                if (jObject != null && jObject.ContainsKey("traceId"))
                {
                    var tmp = jObject["traceId"]!.ToString();
                    if (!tmp.IsNullOrEmpty())
                        TraceId = tmp;
                }
            }

            RabbitLogger = new RabbitLogger(_logger, ConsumerName, Exchange, Route, TraceId);

            try
            {
                if (body.IsNullOrEmpty())
                {
                    RabbitLogger.Error($"received:{body} message body is empty");
                    return ReceivedHandler.Commit;
                }

                RabbitLogger.Info($"received:{body}");

                return await HandlerAsync(body);
            }
            catch (Exception ex)
            {
                RabbitLogger.Error($"handle exception:{ex.GetType().Name}", ex);
                return ReceivedHandler.Commit;
            }
            finally
            {
                RabbitLogger.Info($"consumption completed - {watch.ElapsedMilliseconds}ms");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        protected abstract Task<ReceivedHandler> HandlerAsync(string body);
    }
}
