using Lycoris.Api.Application.Schedule.Shared;
using Lycoris.Api.Core.Logging;
using Lycoris.Quartz.Extensions.Job;

namespace Lycoris.Api.Application.Schedule
{
    /// <summary>
    /// 
    /// </summary>
    [QuartzJob("Cron表达式定时测试任务", Trigger = QuartzTriggerEnum.CRON, Cron = "0 0 0/1 * * ? ")]
    public class CronJobDemo : BaseJob
    {
        public CronJobDemo(ILycorisLoggerFactory factory) : base(factory.CreateLogger<SimpleJobDemo>())
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override Task HandlerWorkAsync()
        {
            this._logger.Info("任务执行");
            this._logger.Warn("任务执行");
            this._logger.Error("任务执行");
            return Task.CompletedTask;
        }
    }
}
