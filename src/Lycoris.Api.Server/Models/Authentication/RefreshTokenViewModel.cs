namespace Lycoris.Api.Server.Models.Authentication
{
    /// <summary>
    /// 
    /// </summary>
    public class RefreshTokenViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public RefreshTokenViewModel() { }

        /// <summary>
        /// 
        /// </summary>
        public RefreshTokenViewModel(string? Token)
        {
            this.Token = Token;
        }

        /// <summary>
        /// 
        /// </summary>
        public string? Token { get; set; }
    }
}
