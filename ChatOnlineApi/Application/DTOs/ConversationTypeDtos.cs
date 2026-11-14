
namespace ChatOnlineApi.Application.DTOs;

public class ConversationTypeDtos
{
    public class AddConversationTypeRequest
    {
        public string? Title { get; set; }
    }

    public class EditConversationTypeRequest
    {
        public int Id { get; set; }
        public string? Title { get; set; }
    }

    public class ShowConversationTypeRequest
    {
        public List<int>? Ids { get; set; }
        public string? Title { get; set; }
    }

    public class ShowConversationTypeResponse
    {
        public int? Id { get; set; }
        public string? Title { get; set; }
    }
}
