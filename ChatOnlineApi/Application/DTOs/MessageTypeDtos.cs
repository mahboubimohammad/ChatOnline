namespace ChatOnlineApi.Application.DTOs;

public static class MessageTypeDtos
{
    public class AddMessageTypeRequest
    {
        public string? Title { get; set; }
    }

    public class EditMessageTypeRequest
    {
        public int Id { get; set; }
        public string? Title { get; set; }
    }

    public class ShowMessageTypeRequest
    {
        public List<int>? Ids { get; set; }
        public string? Title { get; set; }
    }

    public class ShowMessageTypeResponse
    {
        public int? Id { get; set; }
        public string? Title { get; set; }
    }
}