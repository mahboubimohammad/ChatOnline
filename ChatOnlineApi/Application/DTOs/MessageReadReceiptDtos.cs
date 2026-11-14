
namespace ChatOnlineApi.Application.DTOs;

public class MessageReadReceiptDtos
{
    public class AddMessageReadReceiptRequest
    {
        public int MessageId { get; set; }
        public int UserId { get; set; }
    }

    public class EditMessageReadReceiptRequest
    {
        public int Id { get; set; }
        public bool IsRead { get; set; }
    }

    public class ShowMessageReadReceiptRequest
    {
        public List<int>? Ids { get; set; }
        public int? MessageId { get; set; }
        public int? UserId { get; set; }
    }

    public class ShowMessageReadReceiptResponse
    {
        public int? Id { get; set; }
        public int? MessageId { get; set; }
        public int? UserId { get; set; }
        public string? DeliveredAt { get; set; }
        public string? ReadAt { get; set; }
    }
}
