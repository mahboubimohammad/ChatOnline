using SharedLibrary.Common;
using static ChatOnlineApi.Application.DTOs.UserPresenceDtos;

namespace ChatOnlineApi.Application.Contracts;

public interface IUserPresenceService
{
    Task<ServiceResult<object>> GetAllAsync(ShowUserPresenceRequest request,int pageSize,int pageNumber);

    Task<ServiceResult<object>> GetAsync(int userId);

    Task<ServiceResult<object>> UpdatePresenceAsync(UpdateUserPresenceRequest request);
}