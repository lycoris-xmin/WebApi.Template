using System.ComponentModel.DataAnnotations;

namespace Lycoris.Api.Server.Models.Authentication
{
    /// <summary>
    /// 
    /// </summary>
    public class RefreshTokenInput
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string? RefreshToken { get; set; }
    }
}
