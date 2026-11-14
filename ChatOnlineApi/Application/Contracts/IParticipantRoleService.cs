using SharedLibrary.Common;
using SharedLibrary.Models.DTOs;
using static ChatOnlineApi.Application.DTOs.ParticipantRoleDtos;

namespace ChatOnlineApi.Application.Contracts;

public interface IParticipantRoleService
{
    Task<ServiceResult<object>> GetAllAsync(ShowParticipantRoleRequest request,int pageSize,int pageNumber);
    Task<ServiceResult<List<DropDownDTO>>> GetDropDownAsync();
}