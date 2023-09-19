## 小项目api模版

**以.net Webapi作为服务端，适用于小项目快速构建或新手练手使用**

**注意该模版版本为 .net 6**

### 模版安装方式
```shell
dotnet new install Lycoris.Api
```

### 模版使用
```shell
dotnet new Lycoris.Api -n MysqlProject
```

### 模版使用到的Nuget包
- **[Newtonsoft.Json](https://www.newtonsoft.com/json)**
- **[Autofac](https://autofac.org/)**
- **[AutoMapper](https://automapper.org/)**
- **[Pomelo.EntityFrameworkCore.MySql](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql)**
- **[Serilog](https://github.com/serilog/serilog-aspnetcore)**

### 模版使用扩展包
- **[Lycoris.Base - 基础扩展](https://github.com/lycoris-xmin/Lycoris.Base)**
- **[Lycoris.Autofac.Extensions - Autofac扩展](https://github.com/lycoris-xmin/Lycoris.Autofac.Extensions)**
- **[Lycoris.Quartz.Extensions - Quartz调度任务扩展](https://github.com/lycoris-xmin/Lycoris.Quartz.Extensions)**
- **[Lycoris.CSRedisCore.Extensions - CSRedisCore扩展](https://github.com/lycoris-xmin/Lycoris.CSRedisCore.Extensions)**
- **[Lycoris.RabbitMQ.Extensions - Rabbitmq扩展](https://github.com/lycoris-xmin/Lycoris.RabbitMQ.Extensions)**


### 注意事项
**模版使用的数据库为 `Mysql` ，版本必须>= `5.5` 。支持 `8.0` 数据库，由于模版做了表自动引入等相关重复代码操作，故修改其他数据库时，请注释自动引入代码**

```csharp
public class MySqlContext : DbContext
{
    /// <summary>
    /// 实体映射配置
    /// </summary>
    /// <param name="builder"></param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        // 设置数据库表的字符集
        builder.HasCharSet("utf8mb4");

        // 数据库表自动生成注册
        // 非mysql数据库，请注释掉以下部分，或自行改造
        //builder.TableAutoBuilder(this.GetType().Assembly);

        // 执行基类处理
        base.OnModelCreating(builder);
    }
}
```

**模版详细使用方式，请移步至src目录，查看`readme`文件**