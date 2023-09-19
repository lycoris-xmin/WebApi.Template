﻿using Lycoris.Api.Application.RqbbitMq.Impl;
using Lycoris.Base.Logging;
using Lycoris.RabbitMQ.Extensions.Impl;

namespace Lycoris.Api.Application.RqbbitMq.Consumers
{
    public class DemoConsumer : BaseRabbitConsumerListener
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