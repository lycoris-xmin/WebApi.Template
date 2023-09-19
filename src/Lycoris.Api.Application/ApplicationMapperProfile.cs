using AutoMapper;
using Lycoris.Api.Application.AppService.Authentication.Dtos;
using Lycoris.Api.Application.AppService.Permission.Dtos;
using Lycoris.Api.Application.Cached.Dtos;
using Lycoris.Api.EntityFrameworkCore.Tables;
using Lycoris.Api.Model.Configurations;

namespace Lycoris.Api.Application
{
    public class ApplicationMapperProfile : Profile
    {
        public ApplicationMapperProfile()
        {
            AllowNullCollections = true;

            CreateMap<User, LoginValidateDto>();

            CreateMap<LoginValidateDto, LoginDto>();

            CreateMap<LoginDto, LoginUserCacheDto>();

            CreateMap<MenuPermissionChildConfiguration, RoleMenuChildDataDto>();

            CreateMap<MenuPermissionConfiguration, RoleMenuDataDto>();
        }
    }
}
