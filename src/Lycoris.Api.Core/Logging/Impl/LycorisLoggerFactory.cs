using Lycoris.Autofac.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Lycoris.Api.Core.Logging.Impl
{
    [AutofacRegister(ServiceLifeTime.Singleton)]
    public class LycorisLoggerFactory : ILycorisLoggerFactory
    {
        private readonly ILoggerFactory _factory;
        private readonly IHttpContextAccessor _context;

        public LycorisLoggerFactory(ILoggerFactory factory, IHttpContextAccessor context)
        {
            _factory = factory;
            _context = context;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public ILycorisLogger CreateLogger<T>()
        {
            var logger = _factory.CreateLogger<T>();
            return new LycorisLogger(logger, _context);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public ILycorisLogger CreateLogger(Type type)
        {
            var logger = _factory.CreateLogger(type);
            return new LycorisLogger(logger, _context);
        }
    }
}
