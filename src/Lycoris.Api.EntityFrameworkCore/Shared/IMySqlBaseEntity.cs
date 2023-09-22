namespace Lycoris.Api.EntityFrameworkCore.Shared
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMySqlBaseEntity
    {
        /// <summary>
        /// 种子数据
        /// </summary>
        /// <returns></returns>
        List<object> SeedData();
    }
}
