using Lycoris.Api.EntityFrameworkCore.Constants;
using Lycoris.Api.Model.Configurations;

namespace Lycoris.Api.Server.Application
{
    /// <summary>
    /// 
    /// </summary>
    public static class MenuConfigurationBuilder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        public static void AddMenuConfiguration(this IServiceCollection services)
        {
            services.Configure<MenuConfiguration>(x => x.Menus = GetMenuConfiguration());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static List<MenuPermissionConfiguration> GetMenuConfiguration()
        {
            return new List<MenuPermissionConfiguration>()
            {
                SystemMenus()
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static MenuPermissionConfiguration SystemMenus()
        {
            return new MenuPermissionConfiguration()
            {
                MenuName = "系统管理",
                Child = new List<MenuPermissionChildConfiguration>()
                {
                   new MenuPermissionChildConfiguration()
                   {
                        MenuName = "用户管理",
                        Permission = AppPermission.User.Page,
                        Actions = new List<MenuPermissionChildActionConfiguration>()
                        {
                            new MenuPermissionChildActionConfiguration() { ActionName = "查看", Permission = AppPermission.User.View },
                            new MenuPermissionChildActionConfiguration() { ActionName = "查看所有", Permission = AppPermission.User.ViewAll },
                            new MenuPermissionChildActionConfiguration() { ActionName = "新增", Permission = AppPermission.User.Create },
                            new MenuPermissionChildActionConfiguration() { ActionName = "编辑", Permission = AppPermission.User.Update },
                            new MenuPermissionChildActionConfiguration() { ActionName = "删除", Permission = AppPermission.User.Delete },
                            new MenuPermissionChildActionConfiguration() { ActionName = "审核", Permission = AppPermission.User.Audit }
                        }
                   },
                   new MenuPermissionChildConfiguration()
                   {
                        MenuName = "角色管理",
                        Permission = AppPermission.Role.Page,
                        Actions = new List<MenuPermissionChildActionConfiguration>()
                        {
                            new MenuPermissionChildActionConfiguration() { ActionName = "查看", Permission = AppPermission.Role.View },
                            new MenuPermissionChildActionConfiguration() { ActionName = "新增", Permission = AppPermission.Role.Create },
                            new MenuPermissionChildActionConfiguration() { ActionName = "编辑", Permission = AppPermission.Role.Update },
                            new MenuPermissionChildActionConfiguration() { ActionName = "删除", Permission = AppPermission.Role.Delete }
                        }
                   },
                   new MenuPermissionChildConfiguration()
                   {
                        MenuName = "系统设置",
                        Permission = AppPermission.Settings.Page,
                        Actions = new List<MenuPermissionChildActionConfiguration>()
                        {
                            new MenuPermissionChildActionConfiguration() { ActionName = "查看", Permission = AppPermission.Settings.View },
                            new MenuPermissionChildActionConfiguration() { ActionName = "编辑", Permission = AppPermission.Settings.Update }
                        }
                   }
                }
            };
        }
    }
}
