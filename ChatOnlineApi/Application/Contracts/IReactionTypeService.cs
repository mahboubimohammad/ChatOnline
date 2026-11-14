using SharedLibrary.Common;
using SharedLibrary.Models.DTOs;
using static ChatOnlineApi.Application.DTOs.ReactionTypeDtos;

namespace ChatOnlineApi.Application.Contracts;

public interface IReactionTypeService
{
    Task<ServiceResult<object>> GetAllAsync(ShowReactionTypeRequest request,int pageSize,int pageNumber);

    Task<ServiceResult<List<DropDownDTO>>> DropDownAsync();
}