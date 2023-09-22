using Lycoris.Api.EntityFrameworkCore.Common.Attributes;
using Lycoris.Api.EntityFrameworkCore.Constants;
using Lycoris.Api.EntityFrameworkCore.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lycoris.Api.EntityFrameworkCore.Tables
{
    /// <summary>
    /// 角色信息表
    /// </summary>
    [Table("Role")]
    public class Role : MySqlBaseEntity<int>
    {
        /// <summary>
        /// 角色名称
        /// </summary>
        [TableColumn(StringLength = 100, DefaultValue = "", Required = true)]
        public string RoleName { get; set; } = "";

        /// <summary>
        /// 角色用户数
        /// </summary>
        [TableColumn(DefaultValue = 0)]
        public int UserCount { get; set; } = 0;

        /// <summary>
        /// 是否为超级管理员角色
        /// </summary>
        [TableColumn(DefaultValue = false)]
        public bool IsSuperRole { get; set; } = false;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override List<object> SeedData()
        {
            return new List<object>()
            {
                new Role()
                {
                    Id = TableSeedData.RoleData.Id,
                    RoleName = "超级管理员",
                    UserCount = 1,
                    IsSuperRole = true
                }
            };
        }
    }
}
