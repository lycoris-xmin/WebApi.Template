### 项目目录结构

- Lycoris.Api.Application ---- 业务逻辑类库
  - AppService ---- 业务逻辑服务
  - Cached ---- 缓存相关
  - RqbbitMq ---- rabbitmq
  - Schedule ---- quartz 调度任务
  - Shared ---- 通用部分
  - ApplicationMapperProfile.cs  ---- AutoMapper 映射规则配置
  - ApplicationModule.cs         ---- Autofac 扩展注册模块

- Lycoris.Api.Common ---- 通用工具类库
  - Snowflakes ---- 雪花id工具
  - AppSettings ---- appsetting.json配置静态类
  - CommonModule.cs ---- Autofac 扩展注册模块
  
- Lycoris.Api.Core ---- 其他服务类库
  - Cache ---- 缓存服务
  - EntityFrameworkCore ---- EF仓储等相关服务
  - Interceptors ---- APO拦截器
  - Repositories ---- 独立仓储
  - Logging ---- 日志服务
  - CoreModule.cs ---- Autofac 扩展注册模块

- Lycoris.Api.EntityFrameworkCore ---- EFCore类库
  - Common ---- 表自动引入相关等相关服务
  - Constants ---- 数据库相关常量
  - Contexts ---- 数据库上下文
  - Shared ---- 相关基类
  - Tables ---- 数据库表实体
  - DataBaseModule.cs ---- Autofac 扩展注册模块

- Lycoris.Api.Model  ---- 通用实体类库
  - Cnstants ---- 通用常量
  - Configuration ---- 配置实体
  - Contexts ---- 上下文实体
  - Exceptions ---- 扩展异常实体
  - Global ---- 基础入参出参实体基类
  - ModelModule.cs ---- Autofac 扩展注册模块

- Lycoris.Api.Server ---- 程序启动项
  - AppData 
    - EmailTemplate ---- 邮件模版
    - sensitive_words.txt ---- 离线敏感词库
    - logs ---- 日志存储目录
    - *.json ---- ---- 配置文件
  - Application
    - Constants ---- 常量
    - Swaggers ---- swagger配置
    - ApplicationHostedService.cs ---- 程序启动任务
    - MenuConfigurationBuilder.cs ---- 管理后台菜单配置
    - ViewModelMapperProfile.cs ---- AutoMapper 映射规则配置
  - Controllers ---- 控制器
  - FilterAttributes ---- 过滤器
  - Middlewares ---- 中间件
  - Models ---- 接口入参出参实体
  - Shared ---- 基础基类
  - Program.cs ---- 程序入口


### 程序使用须知
- **1. 模版默认使用 `mysql` 作为数据库承载数据**
- **2. 模版默认下不启用 `redis` 、`rabbitmq`,若要启用，请自行填写 `appsettings.json` 文件中 `redis` 及 `mq` 下的相关配置，并将 `use` 选项设置为 `true`**
- **3. 开发环境下默认启用 `swagger`**
- **4. 接口入参出参默认以属性名会转换为驼峰命名形式，若不想使用驼峰命名格式接收和返回数据，请修改 `Program.cs` 相关控制器配置**

### `Lycoris` 系类扩展使用
**`Lycoris.Api.Application` 下对应的扩展目录中均有基础使用示例，足够应付小项目的基础功能，若需要详细配置请移步查看根目录跳转对应的扩展仓库下查看**

### 入参基类

```csharp
/// <summary>
/// 分页请求基类
/// </summary>
public class PageInput
{
    /// <summary>
    /// 页码
    /// </summary>
    [Required, Range(1, int.MaxValue)]
    public int? PageIndex { get; set; }

    /// <summary>
    /// 页面大小
    /// </summary>
    [Required, Range(1, 200)]
    public int? PageSize { get; set; }
}
```

### 出参基类

- **基础基类**
```csharp
/// <summary>
/// 基础响应体
/// </summary>
public class BaseOutput
{
    /// <summary>
    /// 响应code
    /// </summary>
    public ResCodeEnum ResCode { get; set; }

    /// <summary>
    /// 响应信息
    /// </summary>
    public string ResMsg { get; set; } = "";

    /// <summary>
    /// 请求唯一标识码
    /// </summary>
    public string? TraceId { get; set; }
}
```

- **带有实体的基类**
```csharp
/// <summary>
/// 内容响应体
/// </summary>
/// <typeparam name="T"></typeparam>
public class DataOutput<T> : BaseOutput where T : class, new()
{
    /// <summary>
    /// 响应内容
    /// </summary>
    public T? Data { get; set; }
}
```


- **带有列表的基类**
```csharp
/// <summary>
/// 列表响应体
/// </summary>
/// <typeparam name="T"></typeparam>
public class ListOutput<T> : BaseOutput where T : class, new()
{
    /// <summary>
    /// 响应内容
    /// </summary>
    public ListViewModel<T>? Data { get; set; }
}

/// <summary>
/// 列表响应体响应内容
/// </summary>
/// <typeparam name="T"></typeparam>
public class ListViewModel<T> where T : class
{
    /// <summary>
    /// 
    /// </summary>
    public ListViewModel()
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="List"></param>
    public ListViewModel(List<T>? List)
    {
        this.List = List;
    }

    /// <summary>
    /// 列表
    /// </summary>
    public List<T>? List { get; set; }
}
```

- **表数据返回基类**
  
```csharp
/// <summary>
/// 表数据响应体
/// </summary>
/// <typeparam name="T"></typeparam>
public class PageOutput<T> : BaseOutput where T : class, new()
{
    /// <summary>
    /// 响应内容
    /// </summary>
    public PageViewModel<T> Data { get; set; } = new PageViewModel<T>();
}

/// <summary>
/// 表数据响应体 响应内容
/// </summary>
/// <typeparam name="T"></typeparam>
public class PageViewModel<T> where T : class, new()
{
    /// <summary>
    /// 
    /// </summary>
    public PageViewModel()
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Count"></param>
    /// <param name="List"></param>
    public PageViewModel(int Count, List<T>? List)
    {
        this.Count = Count;
        this.List = List;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Count"></param>
    /// <param name="Summary"></param>
    /// <param name="List"></param>
    public PageViewModel(int Count, T? Summary, List<T>? List)
    {
        this.Count = Count;
        this.Summary = Summary;
        this.List = List;
    }

    /// <summary>
    /// 总数
    /// </summary>
    public int Count { get; set; } = 0;

    /// <summary>
    /// 合计
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public T? Summary { get; set; }

    /// <summary>
    /// 列表
    /// </summary>
    public List<T>? List { get; set; }
}
```

### 其他内容后续补充