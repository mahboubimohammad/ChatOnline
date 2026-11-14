
using SharedLibrary.Common;
using static ChatOnlineApi.Application.DTOs.MessageReadReceiptDtos;

namespace ChatOnlineApi.Application.Contracts;

public interface IMessageReadReceiptService
{
    Task<ServiceResult<object>> AddAsync(AddMessageReadReceiptRequest request);
    Task<ServiceResult<object>> EditAsync(EditMessageReadReceiptRequest request);
    Task<ServiceResult<object>> DeleteAsync(int receiptId);
    Task<ServiceResult<object>> GetAllAsync(ShowMessageReadReceiptRequest filter, int pageSize, int pageNumber);
    Task<ServiceResult<object>> GetAsync(int receiptId);
}
