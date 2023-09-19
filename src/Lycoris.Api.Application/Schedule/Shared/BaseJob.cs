using Lycoris.Base.Logging;
using Lycoris.Quartz.Extensions.Job;
using Quartz;

namespace Lycoris.Api.Application.Schedule.Shared
{
    public abstract class BaseJob : BaseQuartzJob
    {
        protected readonly JobLogger _logger;

        public BaseJob(ILycorisLogger logger)
        {
            _logger = new JobLogger(logger);
        }


        protected override Task DoWork(IJobExecutionContext context)
        {
            _logger.JobWorkRegister(JobTraceId, JobName);
            return DoWorkAsync(context);
        }

        protected abstract Task DoWorkAsync(IJobExecutionContext context);
    }
}
