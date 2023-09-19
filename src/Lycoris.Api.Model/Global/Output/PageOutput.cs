using Newtonsoft.Json;

namespace Lycoris.Api.Model.Global.Output
{
    /// <summary>
    /// 表数据响应体
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PageOutput<T> : BaseOutput where T : class, new()
    {
        /// <summary>
        /// 响应内容
        /// </summary>
        public PageViewModel<T> Data { get; set; } = new PageViewModel<T>();
    }

    /// <summary>
    /// 表数据响应体 响应内容
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PageViewModel<T> where T : class, new()
    {
        /// <summary>
        /// 
        /// </summary>
        public PageViewModel()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Count"></param>
        /// <param name="List"></param>
        public PageViewModel(int Count, List<T>? List)
        {
            this.Count = Count;
            this.List = List;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Count"></param>
        /// <param name="Summary"></param>
        /// <param name="List"></param>
        public PageViewModel(int Count, T? Summary, List<T>? List)
        {
            this.Count = Count;
            this.Summary = Summary;
            this.List = List;
        }

        /// <summary>
        /// 总数
        /// </summary>
        public int Count { get; set; } = 0;

        /// <summary>
        /// 合计
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public T? Summary { get; set; }

        /// <summary>
        /// 列表
        /// </summary>
        public List<T>? List { get; set; }
    }
}
