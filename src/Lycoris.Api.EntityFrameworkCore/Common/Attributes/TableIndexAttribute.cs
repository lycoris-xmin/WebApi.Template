namespace Lycoris.Api.EntityFrameworkCore.Common.Attributes
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class TableIndexAttribute : Attribute
    {
        /// <summary>
        /// 索引字段名称列表
        /// </summary>
        public readonly string[] Properties;

        /// <summary>
        /// 是否是唯一索引
        /// </summary>
        public readonly bool IsUnique;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="Property">索引字段名称列表</param>
        /// <param name="IsUnique">是否是唯一索引</param>
        public TableIndexAttribute(string Property, bool IsUnique = false)
        {
            Properties = new string[] { Property };
            this.IsUnique = IsUnique;
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="Properties">索引字段名称列表</param>
        /// <param name="IsUnique">是否是唯一索引</param>
        public TableIndexAttribute(string[] Properties, bool IsUnique = false)
        {
            this.Properties = Properties;
            this.IsUnique = IsUnique;
        }
    }
}
