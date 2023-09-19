namespace Lycoris.Api.Application.AppService.Authentication.Dtos
{
    public class LoginValidateDto
    {
        public long? Id { get; set; }

        public string? NickName { get; set; }

        public string? Avatar { get; set; }

        public int? RoleId { get; set; }
    }
}
