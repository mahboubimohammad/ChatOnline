using SharedLibrary.Common;
using ChatOnlineApi.Application.DTOs;
using static ChatOnlineApi.Application.DTOs.NotificationDtos;

namespace ChatOnlineApi.Application.Contracts;

public interface INotificationService
{
    Task<ServiceResult<object>> AddAsync(AddNotificationRequest request);

    Task<ServiceResult<object>> GetAllAsync(ShowNotificationRequest request,int pageSize,int pageNumber);

    Task<ServiceResult<object>> GetAsync(int id);

    Task<ServiceResult<object>> MarkAsReadAsync(int id);

    Task<ServiceResult<object>> DeleteAsync(int id);
}
