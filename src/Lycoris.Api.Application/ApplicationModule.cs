using Lycoris.Api.Application.RqbbitMq.Constants;
using Lycoris.Api.Application.RqbbitMq.Consumers;
using Lycoris.Api.Common;
using Lycoris.Api.Core.Interceptors.Transactional;
using Lycoris.Autofac.Extensions;
using Lycoris.Autofac.Extensions.Impl;
using Lycoris.RabbitMQ.Extensions;
using Lycoris.RabbitMQ.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Lycoris.Api.Application
{
    public class ApplicationModule : LycorisRegisterModule
    {
        public override void ModuleRegister(LycorisModuleBuilder builder)
        {
            // 为当前程序集注册一个AOP拦截器
            builder.InterceptedBy<TransactionalInterceptor>();
        }

        public override void SerivceRegister(IServiceCollection services)
        {
            // 任务调度
            QuartzSchedulerBuilder(services);

            // MQ
            RabbitMQBuilder(services);
        }

        private static void QuartzSchedulerBuilder(IServiceCollection services)
        {
            // 更多使用方法详见：https://www.nuget.org/packages/Lycoris.Quartz.Extensions/6.1.0#show-readme-container
            //services.AddQuartzSchedulerCenter(builder =>
            //{
            //    builder.AddJob<SimpleJobDemo>();
            //    builder.AddJob<CronJobDemo>();
            //});
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        private static void RabbitMQBuilder(IServiceCollection services)
        {
            if (!AppSettings.RabbitMq.Use)
                return;

            services.AddRabbitMQExtensions(builder =>
            {
                const string defaultMqOptions = "DefaultRabbitMQOption";

                // 基础配置注册
                builder.AddRabbitMQOption(defaultMqOptions, opt =>
                {
                    opt.Hosts = new string[] { AppSettings.RabbitMq.Host };
                    opt.Port = AppSettings.RabbitMq.Port;
                    opt.UserName = AppSettings.RabbitMq.UserName;
                    opt.Password = AppSettings.RabbitMq.Password;
                    opt.VirtualHost = AppSettings.RabbitMq.VirtualHost;
                    opt.AutoDelete = true;
                });

                // 生产者注册
                builder.AddRabbitProducer(RabbitMQProducer.Demo, opt =>
                {
                    opt.UseRabbitOption(defaultMqOptions);
                    opt.InitializeCount = 5;
                    opt.Exchange = RabbitMQExchange.Demo;
                    opt.Type = RabbitExchangeType.Direct;
                    opt.RouteQueues = new RouteQueue[]
                    {
                        new RouteQueue()
                        {
                            Route = RabbitMQRoute.Demo,
                            Queue = RabbitMQQueue.Demo
                        }
                    };
                });

                // 消费者注册
                builder.AddRabbitConsumer(opt =>
                {
                    opt.UseRabbitOption(defaultMqOptions);
                    opt.Type = RabbitExchangeType.Direct;
                    opt.RouteQueues = new RouteQueue[]
                    {
                        new RouteQueue()
                        {
                            Route = RabbitMQRoute.Demo,
                            Queue = RabbitMQQueue.Demo
                        }
                    };
                }).AddListener<DemoConsumer>(RabbitMQExchange.Demo, RabbitMQQueue.Demo);
            });
        }
    }
}