using Lycoris.Api.Application.RqbbitMq.Impl;
using Lycoris.Api.Core.Logging;
using Lycoris.RabbitMQ.Extensions.DataModel;

namespace Lycoris.Api.Application.RqbbitMq.Consumers
{
    public class DemoConsumer : RabbitConsumerListener
    {
        public DemoConsumer(ILycorisLoggerFactory factory) : base(factory.CreateLogger<DemoConsumer>(), "demo test")
        {
        }

        protected override async Task<ReceivedHandler> HandlerAsync(string body)
        {
            Console.WriteLine(body);
            this.RabbitLogger.Info(this.ConsumerName);
            this.RabbitLogger.Info(this.TraceId);
            await Task.CompletedTask;
            return ReceivedHandler.Commit;
        }
    }
}
