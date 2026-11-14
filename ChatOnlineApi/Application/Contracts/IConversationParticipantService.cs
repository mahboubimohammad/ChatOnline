using SharedLibrary.Common;
using static ChatOnlineApi.Application.DTOs.ConversationParticipantDtos;

namespace ChatOnlineApi.Application.Contracts;

public interface IConversationParticipantService
{
    Task<ServiceResult<object>> AddAsync(AddConversationParticipantRequest request);

    Task<ServiceResult<object>> EditAsync(EditConversationParticipantRequest request);

    Task<ServiceResult<object>> DeleteAsync(int participantId);

    Task<ServiceResult<object>> GetAllAsync(ShowConversationParticipantRequest filter,int pageSize,int pageNumber);

    Task<ServiceResult<object>> GetAsync(int participantId);
}
