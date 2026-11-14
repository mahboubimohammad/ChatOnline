namespace ChatOnlineApi.Application.DTOs;

public static class NotificationDtos
{
    public class AddNotificationRequest
    {
        public int UserId { get; set; }
        public int? ConversationId { get; set; }
        public int? MessageId { get; set; }
        public string? Title { get; set; }
        public string? Body { get; set; }
        public string? NotificationType { get; set; }
    }

    public class ShowNotificationRequest
    {
        public List<int>? Ids { get; set; }
        public int? UserId { get; set; }
        public int? ConversationId { get; set; }
        public int? MessageId { get; set; }
        public bool? IsRead { get; set; }
        public string? NotificationType { get; set; }
        public string? Title { get; set; }
    }

    public class ShowNotificationResponse
    {
        public int? Id { get; set; }
        public int? UserId { get; set; }
        public int? ConversationId { get; set; }
        public int? MessageId { get; set; }
        public string? Title { get; set; }
        public string? Body { get; set; }
        public string? NotificationType { get; set; }
        public bool? IsRead { get; set; }
        public string? CreatedAt { get; set; }
        public string? ReadAt { get; set; }
    }
}