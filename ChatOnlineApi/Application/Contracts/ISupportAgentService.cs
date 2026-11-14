using ChatOnlineApi.Application.DTOs;
using SharedLibrary.Common;

namespace ChatOnlineApi.Application.Contracts;

public interface ISupportAgentService
{
    Task<ServiceResult<object>> GetAsync();

    Task<ServiceResult<object>> AddAsync(AddSupportAgentRequest request);
}