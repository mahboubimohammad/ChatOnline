namespace ChatOnlineApi.Application.DTOs;

public class ConversationPinDtos
{
    public class AddConversationPinRequest
    {
        public int ConversationId { get; set; }
        public int MessageId { get; set; }
        public int PinnedBy { get; set; }
    }

    public class ShowConversationPinRequest
    {
        public int? ConversationId { get; set; }
        public int? MessageId { get; set; }
        public int? PinnedBy { get; set; }
    }

    public class ShowConversationPinResponse
    {
        public int? Id { get; set; }
        public int? ConversationId { get; set; }
        public int? MessageId { get; set; }
        public int? PinnedBy { get; set; }
        public string? PinnedAt { get; set; }
    }
}
