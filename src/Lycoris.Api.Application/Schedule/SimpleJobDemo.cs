using Lycoris.Api.Application.Schedule.Shared;
using Lycoris.Api.Core.Logging;
using Lycoris.Quartz.Extensions.Job;
using Quartz;

namespace Lycoris.Api.Application.Schedule
{
    [DisallowConcurrentExecution]  // 只有上一个任务完成才会执行下一次任务
    [PersistJobDataAfterExecution] // 这一次的结果作为值传给下一次
    [QuartzJob("简单定时测试任务", Trigger = QuartzTriggerEnum.SIMPLE, IntervalSecond = 5)]
    public class SimpleJobDemo : BaseJob
    {
        public SimpleJobDemo(ILycorisLoggerFactory factory) : base(factory.CreateLogger<SimpleJobDemo>())
        {

        }

        protected override Task HandlerWorkAsync(IJobExecutionContext context)
        {
            this._logger.Info("任务执行");
            this._logger.Warn("任务执行");
            this._logger.Error("任务执行");
            return Task.CompletedTask;
        }
    }
}
