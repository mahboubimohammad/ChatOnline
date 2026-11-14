using SharedLibrary.Common;
using static ChatOnlineApi.Application.DTOs.ConversationPinDtos;

namespace ChatOnlineApi.Application.Contracts;

public interface IConversationPinService
{
    Task<ServiceResult<object>> AddAsync(AddConversationPinRequest request);

    Task<ServiceResult<object>> DeleteAsync(int pinId);

    Task<ServiceResult<object>> GetAllAsync(ShowConversationPinRequest filter,int pageSize,int pageNumber);

    Task<ServiceResult<object>> GetAsync(int pinId);
}
