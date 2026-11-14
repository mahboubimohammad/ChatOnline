namespace ChatOnlineApi.Application.DTOs;

public class ShowSupportAgentRequest
{
    public int? UserId { get; set; }
    public string? FullName { get; set; }
    public bool? IsActive { get; set; }
}

public class AddSupportAgentRequest
{
    public int? UserId { get; set; }
    public string? FullName { get; set; }
}

public class ShowSupportAgentResponse
{
    public int? Id { get; set; }
    public int? UserId { get; set; }
    public string? FullName { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsOnline { get; set; }
    public string? LastSeenAt { get; set; }
    public string? CreatedAt { get; set; }
}