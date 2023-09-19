namespace Lycoris.Api.EntityFrameworkCore.Constants
{
    /// <summary>
    /// 
    /// </summary>
    public class AppPermission
    {
        /// <summary>
        /// 用户管理
        /// </summary>
        public class User
        {
            /// <summary>
            /// 页面
            /// </summary>
            public const string Page = "System.User.Page";

            /// <summary>
            /// 查看
            /// </summary>
            public const string View = "System.User.View";

            /// <summary>
            /// 查看所有
            /// </summary>
            public const string ViewAll = "System.User.ViewAll";

            /// <summary>
            /// 新增
            /// </summary>
            public const string Create = "System.User.Create";

            /// <summary>
            /// 修改
            /// </summary>
            public const string Update = "System.User.Update";

            /// <summary>
            /// 删除
            /// </summary>
            public const string Delete = "System.User.Delete";

            /// <summary>
            /// 审核
            /// </summary>
            public const string Audit = "System.User.Audit";
        }

        /// <summary>
        /// 角色管理
        /// </summary>
        public class Role
        {
            /// <summary>
            /// 页面
            /// </summary>
            public const string Page = "System.Role.Page";

            /// <summary>
            /// 查看
            /// </summary>
            public const string View = "System.Role.View";

            /// <summary>
            /// 新增
            /// </summary>
            public const string Create = "System.Role.Create";

            /// <summary>
            /// 修改
            /// </summary>
            public const string Update = "System.Role.Update";

            /// <summary>
            /// 删除
            /// </summary>
            public const string Delete = "System.Role.Delete";
        }

        /// <summary>
        /// 系统设置
        /// </summary>
        public class Settings
        {
            /// <summary>
            /// 页面
            /// </summary>
            public const string Page = "System.Settings.Page";

            /// <summary>
            /// 查看
            /// </summary>
            public const string View = "System.Settings.View";

            /// <summary>
            /// 修改
            /// </summary>
            public const string Update = "System.Settings.Update";
        }
    }
}
