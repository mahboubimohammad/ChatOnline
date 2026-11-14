
using SharedLibrary.Common;
using static ChatOnlineApi.Application.DTOs.MessageAttachmentDtos;

namespace ChatOnlineApi.Application.Contracts;

public interface IMessageAttachmentService
{
    Task<ServiceResult<object>> AddAsync(AddMessageAttachmentRequest request);
    Task<ServiceResult<object>> EditAsync(EditMessageAttachmentRequest request);
    Task<ServiceResult<object>> DeleteAsync(int attachmentId);
    Task<ServiceResult<object>> GetAllAsync(ShowMessageAttachmentRequest filter, int pageSize, int pageNumber);
    Task<ServiceResult<object>> GetAsync(int attachmentId);
}
