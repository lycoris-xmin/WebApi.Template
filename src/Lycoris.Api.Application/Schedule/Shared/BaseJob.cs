using Lycoris.Api.Core.Logging;
using Lycoris.Quartz.Extensions.Job;

namespace Lycoris.Api.Application.Schedule.Shared
{
    public abstract class BaseJob : BaseQuartzJob
    {
        protected readonly JobLogger _logger;

        public BaseJob(ILycorisLogger logger)
        {
            _logger = new JobLogger(logger);
        }


        protected override Task DoWorkAsync()
        {
            _logger.JobWorkRegister(JobTraceId, JobName);
            return HandlerWorkAsync();
        }

        protected abstract Task HandlerWorkAsync();
    }
}
