using Lycoris.Api.EntityFrameworkCore.Common.Attributes;

namespace Lycoris.Api.EntityFrameworkCore.Shared
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TPrimary"></typeparam>
    public class MySqlBaseEntity<TPrimary> : IMySqlBaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Snowflake]
        [TableColumn(IsPrimary = true, IsIdentity = true)]
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        public virtual TPrimary Id { get; set; }
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。

        /// <summary>
        /// 乐观锁
        /// </summary>
        [TableColumn(IsRowVersion = true)]
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        public virtual byte[] RowVersion { get; set; }
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。

        /// <summary>
        /// 种子数据
        /// </summary>
        /// <returns></returns>
        public virtual List<object> InitialData() => new();
    }
}
