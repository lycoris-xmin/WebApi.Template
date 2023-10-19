using Lycoris.Common.ConfigurationManager;
using Lycoris.Common.Extensions;
using Lycoris.Common.Helper;

namespace Lycoris.Api.Common
{
    /// <summary>
    /// 程序配置文件
    /// </summary>
    public class AppSettings
    {
        private const string Key = "8A3CD5B9";
        private const string Iv = "20200101";

        private static bool? _IsDebugger = null;

        /// <summary>
        /// 是否开发环境
        /// </summary>
        public static bool IsDebugger
        {
            get
            {
                if (_IsDebugger.HasValue)
                    return _IsDebugger.Value;

                _IsDebugger = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development" || Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "DevelopmentServer";
                return _IsDebugger.Value;
            }
        }

        /// <summary>
        /// 程序文件路径
        /// </summary>
        public class Path
        {

            private static string? _RootPath = null;

            /// <summary>
            /// 
            /// </summary>
            public static string RootPath
            {
                get
                {
                    if (_RootPath != null)
                        return _RootPath;

                    _RootPath = IsDebugger ? Directory.GetCurrentDirectory() : AppContext.BaseDirectory;
                    _RootPath = _RootPath.TrimEnd('/').TrimEnd('\\');
                    return _RootPath;
                }
            }

            /// <summary>
            /// 
            /// </summary>
            public static string WebRootPath
            {
                get
                {
                    if (IsDebugger)
                        return $"{Directory.GetCurrentDirectory().TrimEnd('/').TrimEnd('\\')}/wwwroot";
                    else
                        return $"{RootPath}/wwwroot";
                }
            }

            /// <summary>
            /// AppData文件
            /// </summary>
            private static string AppData { get => $"{RootPath}/AppData"; }

            /// <summary>
            /// 
            /// </summary>
            public static string JsonFile
            {
                get
                {
                    if (!IsDebugger)
                        return $"{AppData}/appsettings.json";


                    var localhost = $"{AppData}/appsettings.Localhost.json";
                    if (File.Exists(System.IO.Path.Combine(Directory.GetCurrentDirectory(), localhost)))
                        return localhost;

                    return $"{AppData}/appsettings.Development.json";
                }
            }

            /// <summary>
            /// 
            /// </summary>
            public static List<string> StaticFilePath { get => new() { "/css", "/fonts", "/global", "/icon", "/images", "/js", "/summernote", "/sweetalert", "/viewsource", "/favicon.ico" }; }

            public class EmailTemplate
            {
                /// <summary>
                /// 
                /// </summary>
                public static string EmailTest => $"{AppData}/EmailTemplate/EmailTest.html";

                /// <summary>
                /// 
                /// </summary>
                public static string EmailValidate => $"{AppData}/EmailTemplate/EmailValidate.html";

                /// <summary>
                /// 
                /// </summary>
                public static string EmailAuthorize => $"{AppData}/EmailTemplate/EmailAuthorize.html";

                /// <summary>
                /// 
                /// </summary>
                public static string ResetPassword => $"{AppData}/EmailTemplate/ResetPassword.html";
            }

            /// <summary>
            /// 
            /// </summary>
            public static string SensitiveWord => $"{AppData}/sensitive_words.txt";
        }

        /// <summary>
        /// 程序启动配置
        /// </summary>
        public class Application
        {
            #region Http服务端口号
            private static int? _HttpPort = null;

            /// <summary>
            /// Http服务端口号
            /// </summary>
            public static int HttpPort
            {
                get
                {
                    if (_HttpPort.HasValue)
                        return _HttpPort.Value;

                    var val = SettingManager.TryGetConfig("Application:HttpPort");

                    if (val.IsNullOrEmpty())
                        _HttpPort = 80;
                    else
                        _HttpPort = int.TryParse(val, out int _val) ? _val : 80;

                    return _HttpPort.Value;
                }
                set
                {
                    _HttpPort = value;
                }
            }
            #endregion

            /// <summary>
            /// 跨域配置
            /// </summary>
            public class Cors
            {
                private static string[]? _Origins { get; set; }

                /// <summary>
                /// 允许来源列表
                /// </summary>
                public static string[] Origins
                {
                    get
                    {
                        if (_Origins != null)
                            return _Origins;

                        var tmp = SettingManager.GetSection<List<string>>("Application:Cors:Origins");
                        _Origins = tmp.HasValue() ? tmp!.ToArray() : Array.Empty<string>();

                        return _Origins;
                    }
                }

                private static string[]? _Methods { get; set; }


                /// <summary>
                /// 允许请求方法
                /// </summary>
                public static string[] Methods
                {
                    get
                    {
                        if (_Methods != null)
                            return _Methods;

                        var tmp = SettingManager.GetSection<List<string>>("Application:Cors:Methods");
                        _Methods = tmp.HasValue() ? tmp!.ToArray() : Array.Empty<string>();

                        return _Methods;
                    }
                }

                private static string[]? _Headers { get; set; }

                /// <summary>
                /// 必要请求头
                /// </summary>
                public static string[] Headers
                {
                    get
                    {
                        if (_Headers != null)
                            return _Headers;

                        var tmp = SettingManager.GetSection<List<string>>("Application:Cors:Headers");
                        _Headers = tmp.HasValue() ? tmp!.ToArray() : Array.Empty<string>();

                        return _Headers;
                    }
                }

                private static bool? _AllowCredentials = null;

                /// <summary>
                /// 允许凭据
                /// </summary>
                public static bool AllowCredentials
                {
                    get
                    {
                        if (_AllowCredentials.HasValue)
                            return _AllowCredentials.Value;

                        var tmp = SettingManager.TryGetConfig("Application:Cors:AllowCredentials");
                        if (!tmp.IsNullOrEmpty())
                            _AllowCredentials = tmp.ToTryBool();

                        _AllowCredentials ??= false;
                        return _AllowCredentials.Value;
                    }
                }
            }
        }

        /// <summary>
        /// 雪花Id生成配置
        /// </summary>
        public class Snowflake
        {
            private static int? _WorkIdLength = null;

            /// <summary>
            /// 工作机器id所占用的长度，最大10，默认10
            /// </summary>
            public static int WorkIdLength
            {
                get
                {
                    if (_WorkIdLength.HasValue)
                        return _WorkIdLength.Value;

                    var val = SettingManager.TryGetConfig("Snowflake:WorkIdLength");

                    if (val.IsNullOrEmpty())
                        _WorkIdLength = 80;
                    else
                        _WorkIdLength = int.TryParse(val, out int _val) ? _val : 80;

                    return _WorkIdLength.Value;
                }
                set { _WorkIdLength = value; }
            }

            private static DateTime? _StartTime = null;

            /// <summary>
            /// 计算起始时间
            /// </summary>
            public static DateTime StartTime
            {
                get
                {
                    if (_StartTime.HasValue)
                        return _StartTime.Value;

                    var val = SettingManager.TryGetConfig("Snowflake:StartTime");

                    _StartTime = val.ToTryDateTime();
                    _StartTime ??= new DateTime(2020, 1, 1);

                    return _StartTime.Value;
                }
            }

            /// <summary>
            /// 用于计算时间戳的开始时间
            /// </summary>
            public DateTime StartTimeStamp { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public bool EnabledSnowflakeBucket { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public int SnowflakeBucketLimit { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public TimeSpan SnowflakeBucketRefreshnterval { get; set; }
        }

        /// <summary>
        /// Sql数据库配置
        /// </summary>
        public class Sql
        {
            private static string DockerName => SettingManager.TryGetConfig("Redis:DockerName") ?? "";

            private static string Server
            {
                get
                {
                    var server = "";
                    if (!DockerName.IsNullOrEmpty())
                        server = DockerName;

                    if (server.IsNullOrEmpty())
                        server = SettingManager.TryGetConfig("Sql:Server");

                    if (server.IsNullOrEmpty())
                        throw new ArgumentNullException("Sql:Server");

                    return server!;
                }
            }

            private static string Port
            {
                get
                {
                    var port = SettingManager.TryGetConfig("Sql:Port");
                    if (port.IsNullOrEmpty())
                        throw new ArgumentNullException("Sql:Port");

                    return port;
                }
            }

            private static string DataBase
            {
                get
                {
                    var database = SettingManager.TryGetConfig("Sql:DataBase");
                    if (database.IsNullOrEmpty())
                        throw new ArgumentNullException("Sql:DataBase");

                    return database;
                }
            }

            private static string UserName
            {
                get
                {
                    var userName = SettingManager.TryGetConfig("Sql:UserName");
                    if (userName.IsNullOrEmpty())
                        throw new ArgumentNullException("Sql:Password");

                    return userName;
                }
            }

            private static string Password
            {
                get
                {
                    var password = SettingManager.TryGetConfig("Sql:Password");
                    if (password.IsNullOrEmpty())
                        throw new ArgumentNullException("Sql:Password");

                    password = SecretHelper.DESDecrypt(password, Key, Iv);
                    return password ?? "";
                }
            }

            private static readonly object ConnectionStringLock = new();
            private static string? _ConnectionString = null;

            /// <summary>
            /// 
            /// </summary>
            public static string Version
            {
                get
                {
                    var version = SettingManager.TryGetConfig("Sql:Version");
                    if (version.IsNullOrEmpty())
                        throw new ArgumentNullException("Sql:Version");

                    return version;
                }
            }

            /// <summary>
            /// sql 链接字符串
            /// </summary>
            public static string ConnectionString
            {
                get
                {
                    lock (ConnectionStringLock)
                    {
                        if (!_ConnectionString.IsNullOrEmpty())
                            return _ConnectionString!;

                        _ConnectionString = $"server={Server};port={Port};database={DataBase};user={UserName};password={Password}";
                        return _ConnectionString;
                    }
                }
            }

            private static string? _TablePrefix = null;

            /// <summary>
            /// redis 服务地址
            /// </summary>
            public static string TablePrefix
            {
                get
                {
                    if (_TablePrefix != null)
                        return _TablePrefix;

                    _TablePrefix = SettingManager.TryGetConfig("Sql:TablePrefix");
                    _TablePrefix ??= "";
                    _TablePrefix = _TablePrefix.Trim();

                    return _TablePrefix;
                }
            }

            public class SeedData
            {
                /// <summary>
                /// 
                /// </summary>
                public static string Account
                {
                    get
                    {
                        var value = SettingManager.TryGetConfig("Sql:SeedData:Account");
                        if (value.IsNullOrEmpty())
                            throw new ArgumentNullException("Sql:SeedData:Account");

                        return value;
                    }
                }

                /// <summary>
                /// 
                /// </summary>
                public static string Password
                {
                    get
                    {
                        var value = SettingManager.TryGetConfig("Sql:SeedData:Password");
                        if (value.IsNullOrEmpty())
                            throw new ArgumentNullException("Sql:SeedData:Password");

                        return value;
                    }
                }

                /// <summary>
                /// 
                /// </summary>
                public static string NickName => SettingManager.TryGetConfig("Sql:SeedData:NickName") ?? "超级管理员";

                /// <summary>
                /// 
                /// </summary>
                public static string DefaultAvatar => SettingManager.TryGetConfig("Sql:SeedData:DefaultAvatar") ?? "";
            }
        }

        /// <summary>
        /// Redis配置
        /// </summary>
        public static class Redis
        {
            private static string DockerName => SettingManager.TryGetConfig("Redis:DockerName") ?? "";

            private static bool? _Use = null;

            /// <summary>
            /// 打印至控制台
            /// </summary>
            public static bool Use
            {
                get
                {
                    if (_Use.HasValue)
                        return _Use.Value;

                    var val = SettingManager.TryGetConfig("Redis:Use");
                    if (!val.IsNullOrEmpty() && bool.TryParse(val, out bool _temp))
                        _Use = _temp;

                    _Use ??= false;
                    return _Use.Value;
                }
            }

            #region redis 服务地址
            private static string? _Host = null;

            /// <summary>
            /// redis 服务地址
            /// </summary>
            public static string Host
            {
                get
                {
                    if (_Host != null)
                        return _Host;

                    if (!DockerName.IsNullOrEmpty())
                        _Host = DockerName;

                    if (_Host.IsNullOrEmpty())
                        _Host = SettingManager.TryGetConfig("Redis:Host");

                    if (_Host.IsNullOrEmpty())
                        throw new ArgumentNullException("Redis:Host");

                    return _Host!;
                }
            }
            #endregion

            #region redis 服务端口号
            private static int? _Port = null;

            /// <summary>
            /// redis 服务端口号
            /// </summary>
            public static int Port
            {
                get
                {
                    if (_Port.HasValue)
                        return _Port.Value;

                    var tmp = SettingManager.TryGetConfig("Redis:Port");
                    _Port = tmp.ToTryInt();
                    if (!_Port.HasValue)
                        throw new ArgumentNullException("Redis:Port");

                    return _Port.Value;
                }
            }
            #endregion

            #region 用户名:(redis 6.0)
            private static string? _UserName = null;

            /// <summary>
            /// redis 服务地址
            /// </summary>
            public static string UserName
            {
                get
                {
                    if (_UserName != null)
                        return _UserName;

                    _UserName = SettingManager.TryGetConfig("Redis:UserName");
                    _UserName ??= "";

                    return _UserName;
                }
            }
            #endregion

            #region redis服务密码
            private static string? _Password = null;

            /// <summary>
            /// redis服务密码
            /// </summary>
            public static string Password
            {
                get
                {
                    if (_Password != null)
                        return _Password;

                    _Password = SettingManager.TryGetConfig("Redis:Password");
                    if (_Password.IsNullOrEmpty())
                        throw new ArgumentNullException("Redis:Password");

                    // Yt@6JhDEoaTZVFAm
                    _Password = SecretHelper.DESDecrypt(_Password, Key, Iv);
                    _Password ??= "";
                    return _Password;
                }
            }
            #endregion

            private static int? _UseDatabase = null;
            /// <summary>
            /// redis 库配置
            /// </summary>
            public static int UseDatabase
            {
                get
                {
                    if (_UseDatabase.HasValue)
                        return _UseDatabase.Value;

                    if (int.TryParse(SettingManager.TryGetConfig("Redis:UseDatabase"), out int val))
                        _UseDatabase = val;

                    _UseDatabase ??= 0;

                    return _UseDatabase.Value;
                }
            }

            #region 是否启用SSL
            private static bool? _SSL;

            /// <summary>
            /// 是否启用SSL
            /// </summary>
            public static bool SSL
            {
                get
                {
                    if (_SSL.HasValue)
                        return _SSL.Value;

                    if (bool.TryParse(SettingManager.TryGetConfig("Redis:SSL"), out bool val))
                        _SSL = val;

                    _SSL ??= false;
                    return _SSL.Value;
                }
            }

            private static string[]? _Sentinels = null;

            /// <summary>
            /// 哨兵集群
            /// </summary>
            public static string[] Sentinels
            {
                get
                {
                    if (_Sentinels != null)
                        return _Sentinels;

                    var temp = SettingManager.TryGetSection<List<string>>("Redis:Sentinels");

                    if (temp.HasValue())
                        _Sentinels = temp!.ToArray();

                    _Sentinels ??= Array.Empty<string>();

                    return _Sentinels;
                }
            }
            #endregion

            #region 超时时间
            private static int? _ConnectTimeout;
            /// <summary>
            /// 超时时间
            /// </summary>
            public static int ConnectTimeout
            {
                get
                {
                    if (_ConnectTimeout.HasValue)
                        return _ConnectTimeout.Value;

                    if (int.TryParse(SettingManager.TryGetConfig("Redis:ConnectTimeout"), out int val))
                        _ConnectTimeout = val;

                    _ConnectTimeout ??= 0;
                    return _ConnectTimeout.Value;
                }
            }

            private static int? _SyncTimeout;

            /// <summary>
            /// 
            /// </summary>
            public static int SyncTimeout
            {
                get
                {
                    if (_SyncTimeout.HasValue)
                        return _SyncTimeout.Value;

                    if (int.TryParse(SettingManager.TryGetConfig("Redis:SyncTimeout"), out int val))
                        _SyncTimeout = val;

                    _SyncTimeout ??= 0;
                    return _SyncTimeout.Value;
                }
            }
            #endregion

            #region Poolsize
            private static int? _Poolsize;

            /// <summary>
            /// 
            /// </summary>
            public static int Poolsize
            {
                get
                {
                    if (_Poolsize.HasValue)
                        return _Poolsize.Value;

                    if (int.TryParse(SettingManager.TryGetConfig("Redis:Poolsize"), out int val))
                        _Poolsize = val;

                    _Poolsize ??= 0;
                    return _Poolsize.Value;
                }
            }
            #endregion

            private static int? _ConnectRetry;

            /// <summary>
            /// 
            /// </summary>
            public static int ConnectRetry
            {
                get
                {
                    if (_ConnectRetry.HasValue)
                        return _ConnectRetry.Value;

                    if (int.TryParse(SettingManager.TryGetConfig("Redis:ConnectRetry"), out int val))
                        _ConnectRetry = val;

                    _ConnectRetry ??= 0;
                    return _ConnectRetry.Value;
                }
            }
        }

        /// <summary>
        /// rabbitmq配置
        /// </summary>
        public static class RabbitMq
        {
            private static bool? _Use = null;

            /// <summary>
            /// 打印至控制台
            /// </summary>
            public static bool Use
            {
                get
                {
                    if (_Use.HasValue)
                        return _Use.Value;

                    var val = SettingManager.TryGetConfig("RabbitMq:Use");
                    if (!val.IsNullOrEmpty() && bool.TryParse(val, out bool _temp))
                        _Use = _temp;

                    _Use ??= false;
                    return _Use.Value;
                }
            }

            private static string? _Host = null;

            /// <summary>
            /// MQ服务IP
            /// </summary>
            public static string Host
            {
                get
                {
                    if (!_Host.IsNullOrEmpty())
                        return _Host!;

                    _Host = SettingManager.TryGetConfig("RabbitMq:Host");
                    if (_Host.IsNullOrEmpty())
                        throw new ArgumentNullException("RabbitMq:Host");

                    return _Host;
                }
            }

            private static int? _Port = null;

            /// <summary>
            /// MQ服务端口号
            /// </summary>
            public static int Port
            {
                get
                {
                    if (_Port.HasValue)
                        return _Port.Value;

                    var val = SettingManager.TryGetConfig("RabbitMq:Port");

                    if (val.IsNullOrEmpty())
                        throw new ArgumentNullException("RabbitMq:Port");

                    if (int.TryParse(val, out int result))
                        _Port = result;
                    else
                        throw new ArgumentException("RabbitMq:Port => Type error");

                    return _Port.Value;
                }
            }

            private static string? _UserName = null;

            /// <summary>
            /// MQ账户
            /// </summary>
            public static string UserName
            {
                get
                {
                    if (string.IsNullOrEmpty(_UserName) == false)
                        return _UserName;

                    _UserName = SettingManager.TryGetConfig("RabbitMq:UserName");
                    if (_UserName.IsNullOrEmpty())
                        throw new ArgumentNullException("RabbitMq:UserName");

                    return _UserName;
                }
            }

            private static string? _Password = null;

            /// <summary>
            /// MQ密码
            /// </summary>
            public static string Password
            {
                get
                {
                    if (string.IsNullOrEmpty(_Password) == false)
                        return _Password;

                    _Password = SettingManager.TryGetConfig("RabbitMq:Password");
                    if (_Password.IsNullOrEmpty())
                        throw new ArgumentNullException("RabbitMq:Password");

                    _Password = SecretHelper.DESDecrypt(_Password, Key, Iv);
                    _Password ??= "";
                    return _Password;
                }
            }

            private static string? _VirtualHost = null;

            /// <summary>
            /// MQ服务虚拟端口
            /// </summary>
            public static string VirtualHost
            {
                get
                {
                    if (string.IsNullOrEmpty(_VirtualHost) == false)
                        return _VirtualHost;

                    _VirtualHost = SettingManager.TryGetConfig("RabbitMq:VirtualHost");
                    _VirtualHost ??= "/";
                    return _VirtualHost;
                }
            }
        }

        /// <summary>
        /// Serilog配置
        /// </summary>
        public static class Serilog
        {
            private static string? _MinLevel = null;

            /// <summary>
            /// 允许记录日志的最小等级
            /// </summary>
            public static string MinLevel
            {
                get
                {
                    if (_MinLevel != null)
                        return _MinLevel;

                    _MinLevel = SettingManager.TryGetConfig("Serilog:MinLevel");
                    _MinLevel ??= "Information";
                    return _MinLevel;
                }
            }

            private static bool? _Console = null;

            /// <summary>
            /// 打印至控制台
            /// </summary>
            public static bool Console
            {
                get
                {
                    if (_Console.HasValue)
                        return _Console.Value;

                    var val = SettingManager.TryGetConfig("Serilog:Console");
                    if (!val.IsNullOrEmpty() && bool.TryParse(val, out bool _temp))
                        _Console = _temp;

                    _Console ??= false;
                    return _Console.Value;
                }
            }

            private static bool? _File = null;

            /// <summary>
            /// 是否开启日志文件记录
            /// </summary>
            public static bool File
            {
                get
                {
                    if (_File.HasValue)
                        return _File.Value;

                    var val = SettingManager.TryGetConfig("Serilog:File");
                    if (!val.IsNullOrEmpty() && bool.TryParse(val, out bool _temp))
                        _File = _temp;

                    _File ??= false;
                    return _File.Value;
                }
            }

            private static List<SerilogOverrideOptions>? _SerilogOverrideOptions { get; set; }

            /// <summary>
            /// 忽略程序集日志列表
            /// </summary>
            public static List<SerilogOverrideOptions> SerilogOverrideOptions
            {
                get
                {
                    if (_SerilogOverrideOptions != null)
                        return _SerilogOverrideOptions;

                    _SerilogOverrideOptions = SettingManager.GetSection<List<SerilogOverrideOptions>>("Serilog:Overrides");
                    _SerilogOverrideOptions ??= new List<SerilogOverrideOptions>();
                    return _SerilogOverrideOptions;
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SerilogOverrideOptions
    {
        /// <summary>
        /// 程序集
        /// </summary>
        public string Source { get; set; } = string.Empty;

        /// <summary>
        /// 允许记录的最小等级
        /// </summary>
        public string MinLevel { get; set; } = string.Empty;
    }
}
