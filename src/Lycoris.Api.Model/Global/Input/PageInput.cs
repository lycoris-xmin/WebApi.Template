using System.ComponentModel.DataAnnotations;

namespace Lycoris.Api.Model.Global.Input
{
    /// <summary>
    /// 分页请求基类
    /// </summary>
    public class PageInput
    {
        /// <summary>
        /// 页码
        /// </summary>
        [Required, Range(1, int.MaxValue)]
        public int? PageIndex { get; set; }

        /// <summary>
        /// 页面大小
        /// </summary>
        [Required, Range(1, 200)]
        public int? PageSize { get; set; }
    }
}
