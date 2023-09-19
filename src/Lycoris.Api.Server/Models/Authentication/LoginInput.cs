using System.ComponentModel.DataAnnotations;

namespace Lycoris.Api.Server.Models.Authentication
{
    /// <summary>
    /// 
    /// </summary>
    public class LoginInput
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string? Account { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string? OathCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool? Remember { get; set; }
    }
}
