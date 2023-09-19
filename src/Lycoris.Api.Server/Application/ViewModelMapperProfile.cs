using AutoMapper;
using Lycoris.Api.Application.AppService.Authentication.Dtos;
using Lycoris.Api.Application.AppService.Permission.Dtos;
using Lycoris.Api.Server.Models.Authentication;
using Lycoris.Api.Server.Models.Permission;

namespace Lycoris.Api.Server.Application
{
    /// <summary>
    /// 
    /// </summary>
    public class ViewModelMapperProfile : Profile
    {
        /// <summary>
        /// 
        /// </summary>
        public ViewModelMapperProfile()
        {
            AllowNullCollections = true;

            CreateMap<LoginDto, LoginViewModel>();

            CreateMap<RoleMenuChildDataDto, RoleMenusChildDataViewModel>();

            CreateMap<RoleMenuDataDto, RoleMenusDataViewModel>();
        }
    }
}
