using Lycoris.Api.Common;
using Lycoris.Autofac.Extensions;
using Lycoris.CSRedisCore.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Lycoris.Api.Core
{
    public class CoreModule : LycorisRegisterModule
    {
        public override void SerivceRegister(IServiceCollection services)
        {
            services.AddMemoryCache();


            if (AppSettings.Redis.Use)
            {
                // CSRedisCore扩展
                CSRedisCoreBuilder.AddSingleRedisInstance(opt =>
                {
                    opt.Host = AppSettings.Redis.Host;
                    opt.Port = AppSettings.Redis.Port;
                    opt.UserName = AppSettings.Redis.UserName;
                    opt.Password = AppSettings.Redis.Password;
                    opt.UseDatabase = AppSettings.Redis.UseDatabase;
                    opt.ConnectTimeout = AppSettings.Redis.ConnectTimeout;
                    opt.SyncTimeout = AppSettings.Redis.SyncTimeout;
                    opt.Poolsize = AppSettings.Redis.Poolsize;
                    opt.SSL = AppSettings.Redis.SSL;
                    opt.RetryOnFailure = AppSettings.Redis.ConnectRetry;
                    opt.UseSentinels(AppSettings.Redis.Sentinels);
                });
            }
        }
    }
}