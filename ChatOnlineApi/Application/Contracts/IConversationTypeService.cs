
using SharedLibrary.Common;
using static ChatOnlineApi.Application.DTOs.ConversationTypeDtos;

namespace ChatOnlineApi.Application.Contracts;

public interface IConversationTypeService
{

    Task<ServiceResult<object>> GetAsync(int conversationTypeId);
}
