namespace Lycoris.Api.Application.AppService.Permission.Dtos
{
    public class RoleMenuDataDto
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
        public List<RoleMenuChildDataDto> Child { get; set; } = new List<RoleMenuChildDataDto>();
    }

    public class RoleMenuChildDataDto
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
