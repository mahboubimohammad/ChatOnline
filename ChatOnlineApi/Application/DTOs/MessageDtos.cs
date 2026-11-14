
namespace ChatOnlineApi.Application.DTOs;

public class MessageDtos
{
    public class AddMessageRequest
    {
        public int ConversationId { get; set; }
        public int SenderId { get; set; }
        public string? Text { get; set; }
        public int MessageTypeId { get; set; }
        public int? ReplyToMessageId { get; set; }
    }

    public class EditMessageRequest
    {
        public int Id { get; set; }
        public string? Text { get; set; }
    }

    public class ShowMessageRequest
    {
        public List<int>? Ids { get; set; }
        public int? ConversationId { get; set; }
        public int? SenderId { get; set; }
        public int? MessageTypeId { get; set; }
        public string? Text { get; set; }
    }

    public class ShowMessageResponse
    {
        public int? Id { get; set; }
        public int? ConversationId { get; set; }
        public int? SenderId { get; set; }
        public string? Text { get; set; }
        public string? CreatedAt { get; set; }
        public int? ReplyToMessageId { get; set; }
        public int? MessageTypeId { get; set; }
        public string? MessageTypeTitle { get; set; }
        public string? DeletedAt { get; set; }
        public string? EditedAt { get; set; }
    }
}
