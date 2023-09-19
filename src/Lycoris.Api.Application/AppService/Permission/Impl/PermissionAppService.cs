using Lycoris.Api.Application.AppService.Permission.Dtos;
using Lycoris.Api.Application.Shared.Impl;
using Lycoris.Api.Core.EntityFrameworkCore;
using Lycoris.Api.EntityFrameworkCore.Constants;
using Lycoris.Api.EntityFrameworkCore.Tables;
using Lycoris.Api.Model.Configurations;
using Lycoris.Autofac.Extensions;
using Lycoris.AutoMapper.Extensions;
using Lycoris.Base.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Lycoris.Api.Application.AppService.Permission.Impl
{
    /// <summary>
    /// 
    /// </summary>
    [AutofacRegister(ServiceLifeTime.Scoped, PropertiesAutowired = true)]
    public class PermissionAppService : ApplicationBaseService, IPermissionAppService
    {
        private readonly IRepository<RolePermission, int> _rolePermission;

        public PermissionAppService(IRepository<RolePermission, int> rolePermission)
        {
            _rolePermission = rolePermission;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="menus"></param>
        /// <returns></returns>
        public async Task<List<RoleMenuDataDto>> GetRoleMenuListAsync(List<MenuPermissionConfiguration> menus)
        {
            var dto = menus.ToMapList<RoleMenuDataDto>();
            if (CurrentUser.RoleId == TableSeedData.RoleData.Id)
                return dto;

            var permissions = await _rolePermission.GetAll().Where(x => x.RoleId == CurrentUser.RoleId).Select(x => x.Permission).ToListAsync();

            dto = menus.Select(x => new RoleMenuDataDto()
            {
                MenuName = x.MenuName,
                Permission = x.Permission,
                Child = x.Child.Where(y => permissions.Contains(y.Permission!)).Select(y => new RoleMenuChildDataDto()
                {
                    Permission = y.Permission,
                    MenuName = y.MenuName
                }).ToList(),
            }).ToList();

            return dto.Where(x => x.Child.HasValue()).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public Task<string[]> GetRolePermissionListAsync(int roleId) => _rolePermission.GetAll().Where(x => x.RoleId == roleId).Select(x => x.Permission).ToArrayAsync();
    }
}
