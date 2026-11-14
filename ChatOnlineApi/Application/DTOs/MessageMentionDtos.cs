
namespace ChatOnlineApi.Application.DTOs;

public class MessageMentionDtos
{
    public class AddMessageMentionRequest
    {
        public int MessageId { get; set; }
        public int UserId { get; set; }
    }

    public class ShowMessageMentionRequest
    {
        public List<int>? Ids { get; set; }
        public int? MessageId { get; set; }
        public int? UserId { get; set; }
    }

    public class ShowMessageMentionResponse
    {
        public int? Id { get; set; }
        public int? MessageId { get; set; }
        public int? UserId { get; set; }
        public string? CreatedAt { get; set; }
    }
}
