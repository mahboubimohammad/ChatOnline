using SharedLibrary.Common;
using ChatOnlineApi.Application.DTOs;
using static ChatOnlineApi.Application.DTOs.MessageTypeDtos;

namespace ChatOnlineApi.Application.Contracts;

public interface IMessageTypeService
{
    Task<ServiceResult<object>> AddAsync(AddMessageTypeRequest request);

    Task<ServiceResult<object>> EditAsync(EditMessageTypeRequest request);

    Task<ServiceResult<object>> DeleteAsync(int id);

    Task<ServiceResult<object>> GetAllAsync(ShowMessageTypeRequest request,int pageSize,int pageNumber);

    Task<ServiceResult<object>> GetAsync(int id);
}