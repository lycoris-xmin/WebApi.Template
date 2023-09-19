namespace Lycoris.Api.EntityFrameworkCore.Common.Attributes
{
    /// <summary>
    /// 雪花编号
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SnowflakeAttribute : Attribute
    {
    }
}
