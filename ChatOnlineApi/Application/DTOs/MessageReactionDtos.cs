
namespace ChatOnlineApi.Application.DTOs;

public class MessageReactionDtos
{
    public class AddMessageReactionRequest
    {
        public int MessageId { get; set; }
        public int UserId { get; set; }
        public int ReactionTypeId { get; set; }
    }

    public class ShowMessageReactionRequest
    {
        public List<int>? Ids { get; set; }
        public int? MessageId { get; set; }
        public int? UserId { get; set; }
        public int? ReactionTypeId { get; set; }
    }

    public class ShowMessageReactionResponse
    {
        public int? Id { get; set; }
        public int? MessageId { get; set; }
        public int? UserId { get; set; }
        public int? ReactionTypeId { get; set; }
        public string? CreatedAt { get; set; }
    }
}
