using Lycoris.Api.EntityFrameworkCore.Contexts;
using Lycoris.Quartz.Extensions;

namespace Lycoris.Api.Server.Application
{
    /// <summary>
    /// 程序启动任务
    /// </summary>
    public class ApplicationHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IQuartzSchedulerCenter _schedulerCenter;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceProvider"></param>
        public ApplicationHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _schedulerCenter = _serviceProvider.GetRequiredService<IQuartzSchedulerCenter>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();

            // 数据库迁移，预热
            await EntityFrameworkCoreWarmUpAsync(scope.ServiceProvider);

            await _schedulerCenter.StartScheduleAsync();
            await _schedulerCenter.ManualRunNonStandbyJobsAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        /// <summary>
        /// 数据库迁移，预热
        /// </summary>
        /// <param name="provider"></param>
        private static Task EntityFrameworkCoreWarmUpAsync(IServiceProvider provider)
        {
            var context = provider.GetRequiredService<MySqlContext>();
            return context.WarmUpAsync();
        }
    }
}
