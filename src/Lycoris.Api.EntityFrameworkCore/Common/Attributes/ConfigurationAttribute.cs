using Lycoris.Api.EntityFrameworkCore.Tables.Enums;

namespace Lycoris.Api.EntityFrameworkCore.Common.Attributes
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class ConfigurationAttribute : Attribute
    {
        /// <summary>
        /// 配置描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 默认值
        /// </summary>
        public string? DefaultValue { get; set; }

        /// <summary>
        /// 默认值
        /// </summary>
        public Type? DefaultObject { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ConfigurationValueTypeEnum ValueType { get; set; } = ConfigurationValueTypeEnum.String;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Description"></param>
        /// <param name="DefaultValue"></param>
        public ConfigurationAttribute(string Description, string DefaultValue)
        {
            this.Description = Description;
            this.DefaultValue = DefaultValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Description"></param>
        /// <param name="DefaultObject"></param>
        public ConfigurationAttribute(string Description, Type DefaultObject)
        {
            this.Description = Description;
            this.DefaultObject = DefaultObject;
            ValueType = ConfigurationValueTypeEnum.Json;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Description"></param>
        /// <param name="DefaultValue"></param>
        /// <param name="ValueType"></param>
        public ConfigurationAttribute(string Description, string DefaultValue, ConfigurationValueTypeEnum ValueType)
        {
            this.Description = Description;
            this.DefaultValue = DefaultValue;
            this.ValueType = ValueType;
        }
    }
}
