namespace Lycoris.Api.EntityFrameworkCore.Common.Attributes
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class TableColumnAttribute : Attribute
    {
        /// <summary>
        /// 主键
        /// 默认为 <see langword="false"/>
        /// </summary>
        public bool IsPrimary { get; set; } = false;

        /// <summary>
        /// 主键自增
        /// 默认为 <see langword="false"/>
        /// </summary>
        public bool IsIdentity { get; set; } = false;

        /// <summary>
        /// 乐观锁
        /// 默认为 <see langword="false"/>
        /// </summary>
        public bool IsRowVersion { get; set; } = false;

        /// <summary>
        /// 字符长度限制
        /// 默认为 <see langword="0"/>
        /// </summary>
        public int StringLength { get; set; } = 0;

        /// <summary>
        /// 列默认值
        /// 默认为 <see langword="null"/>
        /// </summary>
        public object? DefaultValue { get; set; }

        /// <summary>
        /// 是否必填项
        /// 默认为 <see langword="false"/>
        /// </summary>
        public bool Required { get; set; } = false;

        /// <summary>
        /// 映射数据库类型
        /// </summary>
        public string? ColumnType { get; set; }

        /// <summary>
        /// Json列
        /// 实体必须含有无参构造函数
        /// 默认为 <see langword="false"/>
        /// </summary>
        public bool JsonMap { get; set; } = false;

        /// <summary>
        /// 敏感信息加密
        /// 默认为 <see langword="false"/>
        /// </summary>
        public bool Sensitive { get; set; } = false;

        /// <summary>
        /// 密码加密
        /// 默认为 <see langword="false"/>
        /// </summary>
        public bool SqlPassword { get; set; } = false;
    }
}
