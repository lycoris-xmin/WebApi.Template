using Lycoris.Api.EntityFrameworkCore.Common.Attributes;
using Lycoris.Api.EntityFrameworkCore.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lycoris.Api.EntityFrameworkCore.Tables
{
    /// <summary>
    /// 角色权限信息表
    /// </summary>
    [Table("Role_Permission")]
    [TableIndex(new[] { "RoleId", "Permission" }, true)]
    public class RolePermission : MySqlBaseEntity<int>
    {
        /// <summary>
        /// 角色编号
        /// </summary>
        [TableColumn(Required = true, DefaultValue = 0)]
        public int RoleId { get; set; }

        /// <summary>
        /// 权限Id
        /// </summary>
        [TableColumn(Required = true, StringLength = 100)]
        public string Permission { get; set; } = "";
    }
}
