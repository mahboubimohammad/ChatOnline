
using SharedLibrary.Common;
using static ChatOnlineApi.Application.DTOs.ConversationTypeDtos;

namespace ChatOnlineApi.Application.Contracts;

public interface IConversationTypeService
{
    Task<ServiceResult<object>> AddAsync(AddConversationTypeRequest request);

    Task<ServiceResult<object>> EditAsync(EditConversationTypeRequest request);

    Task<ServiceResult<object>> DeleteAsync(int conversationTypeId);

    Task<ServiceResult<object>> GetAllAsync(ShowConversationTypeRequest filter,int pageSize,int pageNumber);

    Task<ServiceResult<object>> GetAsync(int conversationTypeId);
}
