using System.ComponentModel.DataAnnotations;

namespace Lycoris.Api.Model.Global.Input
{
    /// <summary>
    /// 游标请求基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CursorPageInput<T>
    {
        /// <summary>
        /// 游标
        /// </summary>
        [Required]
        public T? Cursor { get; set; }

        /// <summary>
        /// 页面大小
        /// </summary>
        [Required, Range(1, 200)]
        public int? PageSize { get; set; }
    }
}
