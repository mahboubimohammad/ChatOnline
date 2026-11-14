namespace ChatOnlineApi.Application.DTOs;

public static class UserPresenceDtos
{
    public class UpdateUserPresenceRequest
    {
        public int UserId { get; set; }
        public bool IsOnline { get; set; }
    }

    public class ShowUserPresenceRequest
    {
        public List<int>? UserIds { get; set; }
        public bool? IsOnline { get; set; }
    }

    public class ShowUserPresenceResponse
    {
        public int? Id { get; set; }
        public int? UserId { get; set; }
        public bool? IsOnline { get; set; }
        public string? LastSeenAt { get; set; }
        public string? UpdatedAt { get; set; }
    }
}