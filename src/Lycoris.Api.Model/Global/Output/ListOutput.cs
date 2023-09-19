namespace Lycoris.Api.Model.Global.Output
{
    /// <summary>
    /// 列表响应体
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ListOutput<T> : BaseOutput where T : class, new()
    {
        /// <summary>
        /// 响应内容
        /// </summary>
        public ListViewModel<T>? Data { get; set; }
    }

    /// <summary>
    /// 列表响应体响应内容
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ListViewModel<T> where T : class
    {
        /// <summary>
        /// 
        /// </summary>
        public ListViewModel()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="List"></param>
        public ListViewModel(List<T>? List)
        {
            this.List = List;
        }

        /// <summary>
        /// 列表
        /// </summary>
        public List<T>? List { get; set; }
    }
}
