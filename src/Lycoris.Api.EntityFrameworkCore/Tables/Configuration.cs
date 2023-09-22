using Lycoris.Api.EntityFrameworkCore.Common.Attributes;
using Lycoris.Api.EntityFrameworkCore.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lycoris.Api.EntityFrameworkCore.Tables
{
    /// <summary>
    /// 网站配置表
    /// </summary>
    [Table("Configuration")]
    [TableIndex("ConfigId", true)]
    [TableIndex("ConfigName")]
    public class Configuration : MySqlBaseEntity<int>
    {
        /// <summary>
        /// 配置标识
        /// </summary>
        [TableColumn(StringLength = 100)]
        public string ConfigId { get; set; } = string.Empty;

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
        /// 
        /// </summary>
        /// <returns></returns>
        public override List<object> SeedData()
        {
            return new List<object>()
            {

            };
        }
    }
}
