namespace Lycoris.Api.Server.Models.Authentication
{
    /// <summary>
    /// 
    /// </summary>
    public class LoginViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string? NickName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Avatar { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? RoleId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Token { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? RefreshToken { get; set; }
    }
}
