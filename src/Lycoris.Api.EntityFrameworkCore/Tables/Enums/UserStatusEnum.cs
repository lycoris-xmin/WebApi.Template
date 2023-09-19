namespace Lycoris.Api.EntityFrameworkCore.Tables.Enums
{
    /// <summary>
    /// 
    /// </summary>
    public enum UserStatusEnum
    {
        /// <summary>
        /// 未审核
        /// </summary>
        Defalut = 0,
        /// <summary>
        /// 已审核
        /// </summary>
        Audited = 1,
        /// <summary>
        /// 帐号注销
        /// </summary>
        Cancellation = 100
    }
}
