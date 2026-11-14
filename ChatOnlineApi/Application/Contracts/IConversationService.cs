using SharedLibrary.Common;
using static ChatOnlineApi.Application.DTOs.ConversationDtos;

namespace ChatOnlineApi.Application.Contracts;

public interface IConversationService
{
    Task<ServiceResult<object>> AddAsync(AddConversationRequest request);

    Task<ServiceResult<object>> EditAsync(EditConversationRequest request);

    Task<ServiceResult<object>> DeleteAsync(int conversationId);

    Task<ServiceResult<object>> GetAllAsync(ShowConversationRequest filter, int pageSize, int pageNumber);

    Task<ServiceResult<object>> GetAsync(int conversationId);
}