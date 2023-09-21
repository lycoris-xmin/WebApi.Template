using Lycoris.Api.Core.Logging;
using Lycoris.Autofac.Extensions;
using Lycoris.RabbitMQ.Extensions;

namespace Lycoris.Api.Application.RqbbitMq.Impl
{
    [AutofacRegister(ServiceLifeTime.Singleton)]
    public class RabbitProducterService : IRabbitProducterService
    {
        private readonly ILycorisLogger _logger;
        private readonly IRabbitProducerFactory _rabbitFactory;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="rabbitFactory"></param>
        public RabbitProducterService(ILycorisLoggerFactory factory, IRabbitProducerFactory rabbitFactory)
        {
            _logger = factory.CreateLogger<RabbitProducterService>();
            _rabbitFactory = rabbitFactory;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task DemoPublish(string content)
        {

        }
    }
}
