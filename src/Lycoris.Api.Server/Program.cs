using Lycoris.Api.Application;
using Lycoris.Api.Common;
using Lycoris.Api.Core;
using Lycoris.Api.EntityFrameworkCore;
using Lycoris.Api.Model;
using Lycoris.Api.Model.Cnstants;
using Lycoris.Api.Server.Application;
using Lycoris.Api.Server.Application.Constants;
using Lycoris.Api.Server.Application.Swaggers;
using Lycoris.Api.Server.FilterAttributes;
using Lycoris.Api.Server.Middlewares;
using Lycoris.Autofac.Extensions;
using Lycoris.AutoMapper.Extensions;
using Lycoris.Common.ConfigurationManager;
using Lycoris.Common.Extensions;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using Serilog;
using Serilog.Events;
using System.IO.Compression;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// 配置文件
SettingManager.JsonConfigurationInitialization(AppSettings.Path.JsonFile);

// 
builder.Host.ConfigureHostConfiguration(configuration => configuration.AddCommandLine(args));

// 日志组件
builder.Host.UseSerilog((context, config) =>
{
    AppSettings.Serilog.SerilogOverrideOptions.ForEach(OverrideOption => config.MinimumLevel.Override(OverrideOption.Source, OverrideOption.MinLevel.ToEnum<LogEventLevel>()));

    config.MinimumLevel.Is(AppSettings.Serilog.MinLevel.ToEnum<LogEventLevel>());

    if (AppSettings.Serilog.Console)
        config.WriteTo.Console();

    if (AppSettings.IsDebugger)
        config.WriteTo.Debug();

    if (AppSettings.Serilog.File)
    {
        var logPath = AppSettings.IsDebugger
        ? Path.Combine(AppSettings.Path.RootPath, "AppData/logs", "log.txt")
        : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AppData/logs", "log.txt");

        var template = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} - {Level:u3} - {SourceContext:l} - {Message:lj}{NewLine}{Exception}";
        config.WriteTo.File(logPath, outputTemplate: template, rollingInterval: RollingInterval.Day, shared: true, fileSizeLimitBytes: 10 * 1025 * 1024, rollOnFileSizeLimit: true);
    }
});

// 替换系统自带的DI容器为Autofac
builder.UseAutofacExtensions(builder =>
{
    // 模块注册
    builder.AddLycorisRegisterModule<ModelModule>();
    // 模块注册
    builder.AddLycorisRegisterModule<CommonModule>();
    // 模块注册
    builder.AddLycorisRegisterModule<EntityFrameworkCoreModule>();
    // 模块注册
    builder.AddLycorisRegisterModule<CoreModule>();
    // 模块注册
    builder.AddLycorisRegisterModule<ApplicationModule>();
});

// 设置运行端口号
builder.WebHost.UseUrls($"http://*:{AppSettings.Application.HttpPort}");

// AutoMapper注册
builder.Services.AddAutoMapperService(opt => opt.AddMapperProfile<ViewModelMapperProfile>().AddMapperProfile<ApplicationMapperProfile>());

// 注册请求上下文解析
builder.Services.AddHttpContextAccessor();

// 设置允许制定来源的跨域请求
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        if (AppSettings.Application.Cors.Origins.HasValue())
            builder.WithOrigins(AppSettings.Application.Cors.Origins);
        else
            builder.AllowAnyOrigin();

        if (AppSettings.Application.Cors.Methods.HasValue())
            builder.WithMethods(AppSettings.Application.Cors.Methods);
        else
            builder.AllowAnyMethod();

        if (AppSettings.Application.Cors.Headers.HasValue())
            builder.WithHeaders(AppSettings.Application.Cors.Headers);
        else
            builder.AllowAnyHeader();

        builder.AllowCredentials();
    });
});

// 控制器注册
builder.Services.AddControllers(opt =>
{
    // 接口全局异常捕捉
    opt.Filters.Add<ApiExceptionHandlerAttribute>(0);

    // XSS过滤
    opt.Filters.Add<GanssXssFilterAttribute>(1);

    // 请求上下文
    opt.Filters.Add<RequestContextAttribute>(2);
})
.AddNewtonsoftJson(opt =>
{
    // 属性名驼峰处理
    opt.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
    // 设置时间格式
    opt.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
    // 忽略循环引用
    opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
});

// 参数验证失败处理
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        // 获取验证失败的模型字段 
        var errors = context.ModelState.Where(e => e.Value != null && e.Value.Errors.Count > 0).Select(e => e.Value?.Errors?.FirstOrDefault()?.ErrorMessage).ToList();
        context.HttpContext.Items.AddOrUpdate(HttpItems.ResponseBody, string.Join(",", errors));
        return new BadRequestResult();
    };
});

// 设置表单的最大键值
builder.Services.Configure<FormOptions>(options => options.MultipartBodyLengthLimit = 60000000);

// 配置可以同步请求读取流数据
builder.Services.Configure<KestrelServerOptions>(x => x.AllowSynchronousIO = true);

// 响应压缩配置
builder.Services.Configure<BrotliCompressionProviderOptions>(options => options.Level = CompressionLevel.Fastest);
builder.Services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Fastest);
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
    // 扩展一些类型 (MimeTypes中有一些基本的类型,可以打断点看看)
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "text/html; charset=utf-8", "application/xhtml+xml", "application/atom+xml", "image/svg+xml" });
});

// 开发环境 OpenApi
if (AppSettings.IsDebugger)
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(opt =>
    {
        opt.OperationFilter<SwaggerOperationFilter>();
        opt.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme()
        {
            Name = HttpHeaders.Authentication,
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Description = "访问令牌"
        });

        // 此处仅展示根据版本分组的方法，实际上请根据项目自行改动
        opt.SwaggerDoc(ApiVersionGroup.V1, new Microsoft.OpenApi.Models.OpenApiInfo() { Title = "Lycoris.Api", Version = ApiVersionGroup.V1 });
        // 如果有多个版本免，则需要分别配置每个版本的信息
        //opt.SwaggerDoc(ApiVersionGroup.V2, new Microsoft.OpenApi.Models.OpenApiInfo() { Title = "Lycoris.Api", Version = ApiVersionGroup.V2 });

        // 注释文档
        var source = Assembly.GetEntryAssembly()?.GetName().Name?.Replace(".Server", "");
        opt.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{source}.Server.xml"));
        opt.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{source}.Model.xml"));
    });
}

// 添加程序启动任务
builder.Services.AddHostedService<ApplicationHostedService>();

// 后台菜单
builder.Services.AddMenuConfiguration();

var app = builder.Build();

// 使用AutoMapper全局扩展
app.UseAutoMapperExtensions();

// 开发环境 OpenApi
if (AppSettings.IsDebugger)
{
    app.UseSwagger();
    app.UseSwaggerUI(x =>
    {
        // 此处仅展示根据版本分组的方法，实际上请根据项目自行改动
        x.SwaggerEndpoint($"/swagger/{ApiVersionGroup.V1}/swagger.json", $"Lycoris.Api - {ApiVersionGroup.V1}");
        // 如果有多个版本免，则需要分别配置每个版本的信息
        //x.SwaggerEndpoint($"/swagger/{ApiVersionGroup.V2}/swagger.json", $"Lycoris.Api - {ApiVersionGroup.V2}");
    });
}

// 中间件异常捕捉
app.UseMiddleware<ExceptionHandlerMiddleware>();

// 日志记录中间件
app.UseMiddleware<HttpLoggingMiddleware>();

// 响应压缩
app.UseResponseCompression();

// 路由
app.UseRouting();

// 跨域中间件
app.UseCors();

// Cookie策略
app.UseCookiePolicy(new CookiePolicyOptions() { HttpOnly = HttpOnlyPolicy.Always });

// 终结点设置
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

// 启动程序
app.Run();

