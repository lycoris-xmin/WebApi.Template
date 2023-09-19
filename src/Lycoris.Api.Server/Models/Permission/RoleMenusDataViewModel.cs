namespace Lycoris.Api.Server.Models.Permission
{
    /// <summary>
    /// 
    /// </summary>
    public class RoleMenusDataViewModel
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
        /// 
        /// </summary>
        public List<RoleMenusChildDataViewModel> Child { get; set; } = new List<RoleMenusChildDataViewModel>();
    }

    /// <summary>
    /// 
    /// </summary>
    public class RoleMenusChildDataViewModel
    {
        /// <summary>
        /// 菜单标识
        /// </summary>
        public string? Permission { get; set; }

        /// <summary>
        /// 菜单名称
        /// </summary>
        public string? MenuName { get; set; }
    }
}
