namespace ChatOnlineApi.Application.DTOs;

public static class ReactionTypeDtos
{
    public class ShowReactionTypeRequest
    {
        public List<int>? Ids { get; set; }
        public string? Title { get; set; }
    }

    public class ShowReactionTypeResponse
    {
        public int? Id { get; set; }
        public string? Title { get; set; }
    }
}
