
using SharedLibrary.Common;
using static ChatOnlineApi.Application.DTOs.MessageMentionDtos;

namespace ChatOnlineApi.Application.Contracts;

public interface IMessageMentionService
{
    Task<ServiceResult<object>> AddAsync(AddMessageMentionRequest request);
    Task<ServiceResult<object>> DeleteAsync(int mentionId);
    Task<ServiceResult<object>> GetAllAsync(ShowMessageMentionRequest filter, int pageSize, int pageNumber);
    Task<ServiceResult<object>> GetAsync(int mentionId);
}
