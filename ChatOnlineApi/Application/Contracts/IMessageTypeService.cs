using ChatOnlineApi.Application.DTOs;
using SharedLibrary.Common;
using SharedLibrary.Models.DTOs;
using static ChatOnlineApi.Application.DTOs.MessageTypeDtos;

namespace ChatOnlineApi.Application.Contracts;

public interface IMessageTypeService
{
    Task<ServiceResult<object>> GetAllAsync();
    Task<ServiceResult<List<DropDownDTO>>> DropDownAsync();
}