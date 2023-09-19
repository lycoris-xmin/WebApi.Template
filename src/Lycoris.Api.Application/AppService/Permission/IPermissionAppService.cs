using Lycoris.Api.Application.AppService.Permission.Dtos;
using Lycoris.Api.Application.Shared;
using Lycoris.Api.Model.Configurations;

namespace Lycoris.Api.Application.AppService.Permission
{
    public interface IPermissionAppService : IApplicationBaseService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="menus"></param>
        /// <returns></returns>
        Task<List<RoleMenuDataDto>> GetRoleMenuListAsync(List<MenuPermissionConfiguration> menus);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        Task<string[]> GetRolePermissionListAsync(int roleId);
    }
}
