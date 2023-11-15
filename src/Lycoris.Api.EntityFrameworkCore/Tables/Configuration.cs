using Lycoris.Api.EntityFrameworkCore.Common.Attributes;
using Lycoris.Api.EntityFrameworkCore.Constants;
using Lycoris.Api.EntityFrameworkCore.Shared;
using Lycoris.Api.EntityFrameworkCore.Tables.Enums;
using Lycoris.Common.Extensions;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Lycoris.Api.EntityFrameworkCore.Tables
{
    /// <summary>
    /// 网站配置表
    /// </summary>
    [Table("Configuration")]
    public class Configuration : MySqlBaseEntity<string>
    {
        /// <summary>
        /// 配置名称
        /// </summary>
        [TableColumn(StringLength = 100)]
        public string ConfigName { get; set; } = string.Empty;

        /// <summary>
        /// 配置值
        /// </summary>
        public string Value { get; set; } = "";

        /// <summary>
        /// 配置保存格式
        /// </summary>
        [TableColumn(DefaultValue = ConfigurationValueTypeEnum.String)]
        public ConfigurationValueTypeEnum ValueType { get; set; } = ConfigurationValueTypeEnum.String;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override List<object> SeedData() => GetConfiguration(typeof(AppConfig).GetFields());

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieids"></param>
        /// <returns></returns>
        private static List<object> GetConfiguration(FieldInfo[]? fieids)
        {
            var list = new List<object>();

            if (fieids != null && fieids.Length > 0)
            {
                foreach (var fieid in fieids)
                {
                    var attr = ((ConfigurationAttribute?)Attribute.GetCustomAttribute(fieid, typeof(ConfigurationAttribute)));
                    if (attr == null)
                        continue;

                    list.Add(new Configuration()
                    {
                        Id = (string?)fieid.GetRawConstantValue() ?? "",
                        ConfigName = attr.Description,
                        ValueType = attr.ValueType,
                        Value = attr.DefaultObject != null ? Activator.CreateInstance(attr.DefaultObject).ToJson(new JsonSerializerSettings()
                        {
                            NullValueHandling = NullValueHandling.Include,
                            DateFormatString = "yyyy-MM-dd HH:mm:ss",
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        }) : attr.DefaultValue ?? "",
                    });
                }
            }

            return list;
        }
    }
}
