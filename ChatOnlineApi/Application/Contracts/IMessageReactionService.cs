
using SharedLibrary.Common;
using static ChatOnlineApi.Application.DTOs.MessageReactionDtos;

namespace ChatOnlineApi.Application.Contracts;

public interface IMessageReactionService
{
    Task<ServiceResult<object>> AddAsync(AddMessageReactionRequest request);
    Task<ServiceResult<object>> DeleteAsync(int reactionId);
    Task<ServiceResult<object>> GetAllAsync(ShowMessageReactionRequest filter, int pageSize, int pageNumber);
    Task<ServiceResult<object>> GetAsync(int reactionId);
}
