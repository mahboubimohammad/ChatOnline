namespace ChatOnlineApi.Application.DTOs;

public static class ParticipantRoleDtos
{
    public class ShowParticipantRoleRequest
    {
        public List<int>? Ids { get; set; }
        public string? Title { get; set; }
    }

    public class ShowParticipantRoleResponse
    {
        public int? Id { get; set; }
        public string? Title { get; set; }
    }
}