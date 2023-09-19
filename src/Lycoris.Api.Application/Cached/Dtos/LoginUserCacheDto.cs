namespace Lycoris.Api.Application.Cached.Dtos
{
    public class LoginUserCacheDto
    {
        public long Id { get; set; }

        public string? NickName { get; set; }

        public string? Avatar { get; set; }

        public int? RoleId { get; set; }

        public DateTime TokenExpireTime { get; set; }
    }
}
