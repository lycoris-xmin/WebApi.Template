using System.ComponentModel.DataAnnotations;

namespace Lycoris.Api.Server.Models.Authentication
{
    /// <summary>
    /// 
    /// </summary>
    public class LoginValidateInput
    {
        /// <summary>
        /// 帐号
        /// </summary>
        [Required]
        public string? Account { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Required]
        public string? Password { get; set; }
    }
}
