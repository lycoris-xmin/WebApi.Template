namespace Lycoris.Api.Common.Snowflakes
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISnowflakesMaker
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="workId"></param>
        /// <returns></returns>
        long GetSnowflakesId(int? workId = null);
    }
}
