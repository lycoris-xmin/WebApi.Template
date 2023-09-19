namespace Lycoris.Api.Server.Models.Authentication
{
    /// <summary>
    /// 
    /// </summary>
    public class LoginValidateViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public LoginValidateViewModel() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="OathCode"></param>
        public LoginValidateViewModel(string? OathCode)
        {
            this.OathCode = OathCode;
        }

        /// <summary>
        /// 授权码
        /// </summary>
        public string? OathCode { get; set; }
    }
}
