using Lycoris.Api.Common;
using Lycoris.Api.Common.Snowflakes.Impl;
using Lycoris.Api.EntityFrameworkCore.Common.Attributes;
using Lycoris.Api.EntityFrameworkCore.Shared;
using Lycoris.Api.EntityFrameworkCore.Tables.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lycoris.Api.EntityFrameworkCore.Tables
{
    /// <summary>
    /// 用户信息表
    /// </summary>
    [Table("User")]
    [TableIndex(new[] { "Account" }, true)]
    [TableIndex(new[] { "RoleId" })]
    public class User : MySqlBaseEntity<long>
    {
        /// <summary>
        /// 帐号
        /// </summary>
        [TableColumn(StringLength = 100, Required = true)]
        public string Account { get; set; } = "";

        /// <summary>
        /// 密码
        /// </summary>
        [TableColumn(StringLength = 100, Required = true, SqlPassword = true)]
        public string Password { get; set; } = "";

        /// <summary>
        /// 用户昵称
        /// </summary>
        [TableColumn(StringLength = 30)]
        public string NickName { get; set; } = "";

        /// <summary>
        /// 头像
        /// </summary>
        [TableColumn(StringLength = 30)]
        public string Avatar { get; set; } = "";

        /// <summary>
        /// 用户状态 0-未审核，1-已审核，100-帐号注销
        /// </summary>
        [TableColumn(Required = true, DefaultValue = UserStatusEnum.Defalut)]
        public UserStatusEnum Status { get; set; }

        /// <summary>
        /// 角色编号
        /// </summary>
        [TableColumn(DefaultValue = 0, Required = true)]
        public int RoleId { get; set; } = 0;

        /// <summary>
        /// 是否为超级管理员
        /// </summary>
        [TableColumn(DefaultValue = false)]
        public bool IsSuperAdmin { get; set; } = false;

        /// <summary>
        /// 创建用户
        /// </summary>
        [TableColumn(Required = true, DefaultValue = 0)]
        public long CreateUserId { get; set; } = 0;

        /// <summary>
        /// 创建时间
        /// </summary>
        [TableColumn(Required = true)]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 种子数据
        /// </summary>
        /// <returns></returns>
        public override List<object> InitialData()
        {
            return new List<object>()
            {
                new User()
                {
                    Id = new SnowflakesMaker().GetSnowflakesId(),
                    Account = AppSettings.Sql.SeedData.Account,
                    Password = AppSettings.Sql.SeedData.Password,
                    NickName = AppSettings.Sql.SeedData.NickName,
                    Avatar = AppSettings.Sql.SeedData.DefaultAvatar,
                    CreateUserId = 0,
                    RoleId = 1,
                    IsSuperAdmin = true,
                    CreateTime = DateTime.Now
                }
            };
        }
    }
}
