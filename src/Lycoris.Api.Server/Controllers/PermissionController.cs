using Lycoris.Api.Application.AppService.Permission;
using Lycoris.Api.Model.Configurations;
using Lycoris.Api.Model.Global.Output;
using Lycoris.Api.Server.FilterAttributes;
using Lycoris.Api.Server.Models.Permission;
using Lycoris.Api.Server.Shared;
using Lycoris.AutoMapper.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Lycoris.Api.Server.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]")]
    public class PermissionController : BaseController
    {
        private readonly MenuConfiguration _configuration;
        private readonly IPermissionAppService _permission;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="permission"></param>
        public PermissionController(IOptions<MenuConfiguration> configuration, IPermissionAppService permission)
        {
            _configuration = configuration.Value;
            _permission = permission;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("Role/Menus")]
        [AppAuthentication]
        public async Task<ListOutput<RoleMenusDataViewModel>> RoleMenus()
        {
            var dto = await _permission.GetRoleMenuListAsync(this._configuration.Menus);
            return Success(dto.ToMapList<RoleMenusDataViewModel>());
        }
    }
}
