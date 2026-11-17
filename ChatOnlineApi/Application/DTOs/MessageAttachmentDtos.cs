using Microsoft.AspNetCore.Http;

namespace ChatOnlineApi.Application.DTOs;

public class MessageAttachmentDtos
{
    public class AddMessageAttachmentRequest
    {
        public int MessageId { get; set; }
        public List<IFormFile>? Files { get; set; }
    }

    public class EditMessageAttachmentRequest
    {
        public int Id { get; set; }
        public string? FileName { get; set; }
    }

    public class ShowMessageAttachmentRequest
    {
        public List<int>? Ids { get; set; }
        public int? MessageId { get; set; }
        public string? FileName { get; set; }
        public string? ContentType { get; set; }
    }

    public class ShowMessageAttachmentResponse
    {
        public int? Id { get; set; }
        public int? MessageId { get; set; }
        public string? FileUrl { get; set; }
        public string? FileName { get; set; }
        public long? FileSize { get; set; }
        public string? ContentType { get; set; }
        public string? CreatedAt { get; set; }
    }
}