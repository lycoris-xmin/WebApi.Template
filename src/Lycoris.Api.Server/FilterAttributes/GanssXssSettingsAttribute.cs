namespace Lycoris.Api.Server.FilterAttributes
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public sealed class GanssXssSettingsAttribute : Attribute
    {
        /// <summary>
        /// XSS过滤属性白名单
        /// </summary>
        public string Attributes { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public string Schemes { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public string Tags { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public string CssProperties { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public string UriAttributes { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public readonly string[] IgnoreProperties;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="IgnoreProperties"></param>
        public GanssXssSettingsAttribute(params string[] IgnoreProperties)
        {
            this.IgnoreProperties = IgnoreProperties ?? Array.Empty<string>();
        }
    }
}
