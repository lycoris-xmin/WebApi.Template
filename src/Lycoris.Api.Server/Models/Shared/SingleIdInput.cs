using System.ComponentModel.DataAnnotations;

namespace Lycoris.Api.Server.Models.Shared
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SingleIdInput<T>
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public T? Id { get; set; }
    }
}
