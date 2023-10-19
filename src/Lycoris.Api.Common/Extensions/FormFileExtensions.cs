using Lycoris.Api.Common.Extensions;
using Lycoris.Common.Extensions;
using Microsoft.AspNetCore.Http;

namespace Lycoris.Api.Common.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class FormFileExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static void SaveAs(this IFormFile file, string path) => file.OpenReadStream().SaveAs(path);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static async Task SaveAsAsync(this IFormFile file, string path) => await file.OpenReadStream().SaveAsAsync(path);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static void SaveAs(this IFormFile file, string path, string fileName) => file.OpenReadStream().SaveAs(path, fileName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static async Task SaveAsAsync(this IFormFile file, string path, string fileName) => await file.OpenReadStream().SaveAsAsync(path, fileName);
    }
}
