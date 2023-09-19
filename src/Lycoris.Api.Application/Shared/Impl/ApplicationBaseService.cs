using Lycoris.Api.Core.Repositories;
using Lycoris.Api.Model.Contexts;
using Lycoris.Base.Logging;

namespace Lycoris.Api.Application.Shared.Impl
{
    public class ApplicationBaseService : IApplicationBaseService
    {
        /// <summary>
        /// 
        /// </summary>
        public Lazy<ILycorisLoggerFactory>? LycorisLoggerFactory { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private ILycorisLogger? logger = null;

        /// <summary>
        /// 
        /// </summary>
        protected virtual ILycorisLogger _logger
        {
            get
            {
                this.logger ??= LycorisLoggerFactory!.Value.CreateLogger(this.GetType());
                return this.logger;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Lazy<RequestContext>? RequestContext { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Lazy<IConfigurationRepository>? ApplicationConfiguration { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected virtual RequestContext CurrentRequest { get => this.RequestContext!.Value; }

        /// <summary>
        /// 
        /// </summary>
        protected virtual RequestUserContext CurrentUser { get => this.RequestContext!.Value.User ?? new RequestUserContext(); }

        /// <summary>
        /// 
        /// </summary>
        protected virtual bool LoginState { get => CurrentUser.Id > 0; }
    }
}
