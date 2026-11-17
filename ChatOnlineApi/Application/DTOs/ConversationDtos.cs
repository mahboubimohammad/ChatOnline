
namespace ChatOnlineApi.Application.DTOs;

public class ConversationDtos
{
    public class AddConversationRequest
    {
        public int? UserId { get; set; }
        public int? SupportAgentId { get; set; }
        public string? SupportAgentName { get; set; }
        public string? CreatedByName { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
    }

    public class EditConversationRequest
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }

    public class ShowConversationRequest
    {
        public List<int>? Ids { get; set; }
        public int? UserId { get; set; }
        public int? SupportAgentId { get; set; }
        public string? Title { get; set; }
        public bool? IsActive { get; set; }
    }

    public class ShowConversationResponse
    {
        public int? Id { get; set; }
        public int? UserId { get; set; }
        public int? SupportAgentId { get; set; }
        public string? SupportAgentName { get; set; }
        public string? CreatedByName { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? CreatedAt { get; set; }
        public string? LastMessageAt { get; set; }
        public bool? IsActive { get; set; }
    }
}