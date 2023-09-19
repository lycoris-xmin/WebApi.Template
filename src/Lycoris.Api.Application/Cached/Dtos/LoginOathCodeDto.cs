using Lycoris.Api.Application.AppService.Authentication.Dtos;

namespace Lycoris.Api.Application.Cached.Dtos
{
    public class LoginOathCodeDto
    {

        public LoginOathCodeDto(string? OathCode, LoginValidateDto? Value)
        {
            this.OathCode = OathCode;
            this.Value = Value;
        }

        /// <summary>
        /// 
        /// </summary>
        public string? OathCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public LoginValidateDto? Value { get; set; }
    }
}
