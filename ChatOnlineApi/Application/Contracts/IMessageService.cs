
using SharedLibrary.Common;
using static ChatOnlineApi.Application.DTOs.MessageDtos;

namespace ChatOnlineApi.Application.Contracts;

public interface IMessageService
{
    Task<ServiceResult<object>> AddAsync(AddMessageRequest request);
    Task<ServiceResult<object>> EditAsync(EditMessageRequest request);
    Task<ServiceResult<object>> DeleteAsync(int messageId);
    Task<ServiceResult<object>> GetAllAsync(ShowMessageRequest filter, int pageSize, int pageNumber);
    Task<ServiceResult<object>> GetAsync(int messageId);
}
