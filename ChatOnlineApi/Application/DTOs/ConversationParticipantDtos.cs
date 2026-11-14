
namespace ChatOnlineApi.Application.DTOs;

public class ConversationParticipantDtos
{
    public class AddConversationParticipantRequest
    {
        public int ConversationId { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
    }

    public class EditConversationParticipantRequest
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public bool IsActive { get; set; }
        public bool IsMuted { get; set; }
        public bool IsPinned { get; set; }
        public bool IsArchived { get; set; }
    }

    public class ShowConversationParticipantRequest
    {
        public int? ConversationId { get; set; }
        public int? UserId { get; set; }
        public int? RoleId { get; set; }
        public bool? IsActive { get; set; }
    }

    public class ShowConversationParticipantResponse
    {
        public int? Id { get; set; }
        public int? ConversationId { get; set; }
        public int? UserId { get; set; }
        public int? RoleId { get; set; }
        public string? JoinedAt { get; set; } 
        public string? LeftAt { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsMuted { get; set; }
        public bool? IsPinned { get; set; }
        public bool? IsArchived { get; set; }
    }
}
