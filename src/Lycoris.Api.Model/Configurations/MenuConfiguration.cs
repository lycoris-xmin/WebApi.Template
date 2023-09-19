namespace Lycoris.Api.Model.Configurations
{
    /// <summary>
    /// 
    /// </summary>
    public class MenuConfiguration
    {
        /// <summary>
        /// 
        /// </summary>
        public List<MenuPermissionConfiguration> Menus { get; set; } = new List<MenuPermissionConfiguration>();
    }

    /// <summary>
    /// 
    /// </summary>
    public class MenuPermissionConfiguration
    {
        /// <summary>
        /// 菜单标识
        /// </summary>
        public string? Permission { get; set; }

        /// <summary>
        /// 菜单名称
        /// </summary>
        public string? MenuName { get; set; }

        /// <summary>
        /// 二级菜单
        /// </summary>
        public List<MenuPermissionChildConfiguration> Child { get; set; } = new List<MenuPermissionChildConfiguration>();
    }

    /// <summary>
    /// 
    /// </summary>
    public class MenuPermissionChildConfiguration
    {
        /// <summary>
        /// 权限标识
        /// </summary>
        public string? Permission { get; set; }

        /// <summary>
        /// 权限名称
        /// </summary>
        public string? MenuName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<MenuPermissionChildActionConfiguration> Actions { get; set; } = new List<MenuPermissionChildActionConfiguration>();
    }

    /// <summary>
    /// 
    /// </summary>
    public class MenuPermissionChildActionConfiguration
    {
        /// <summary>
        /// 权限标识
        /// </summary>
        public string? Permission { get; set; }

        /// <summary>
        /// 权限名称
        /// </summary>
        public string? ActionName { get; set; }
    }
}
