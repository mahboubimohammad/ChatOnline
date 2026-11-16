
using ChatOnlineApi.Application.Contracts;
using ChatOnlineApi.Domain.Base;
using ChatOnlineApi.Domain.Entities;
using ChatOnlineApi.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedLibrary.Common;
using static ChatOnlineApi.Application.DTOs.ConversationTypeDtos;

namespace ChatOnlineApi.Application.Services;

public class ConversationTypeService(
    IUnitOfWork _unitOfWork,
    IConversationTypeRepository _conversationTypes,
    IConversationRepository _conversations,
    ILogger<ConversationTypeService> _logger) : IConversationTypeService
{

    public async Task<ServiceResult<object>> GetAsync(int conversationTypeId)
    {
        try
        {
            if (conversationTypeId <= 0)
                return ServiceResult<object>.Fail("شناسه معتبر نیست", 400);

            var conversationType = await _conversationTypes
                .Get(x => x.Id == conversationTypeId)
                .FirstOrDefaultAsync();

            if (conversationType == null)
                return ServiceResult<object>.Fail("نوع گفتگو پیدا نشد", 404);

            var response = new ShowConversationTypeResponse
            {
                Id = conversationType.Id,
                Title = conversationType.Title
            };

            return ServiceResult<object>.Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetConversationType");

            return ServiceResult<object>.Fail(
                "خطای داخلی سرور رخ داده است", 500, ex.ToString());
        }
    }
}
