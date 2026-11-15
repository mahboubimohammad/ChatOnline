using ChatOnlineApi.Application.Contracts;
using ChatOnlineApi.Domain.Base;
using ChatOnlineApi.Domain.Entities;
using ChatOnlineApi.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedLibrary.Common;
using SharedLibrary.Helpers;
using static ChatOnlineApi.Application.DTOs.ConversationDtos;

namespace ChatOnlineApi.Application.Services;

public class ConversationService(
    IUnitOfWork _unitOfWork,
    IConversationRepository _conversations,
    ILogger<ConversationService> _logger) : IConversationService
{
    public async Task<ServiceResult<object>> AddAsync(AddConversationRequest request)
    {
        try
        {
            if (request == null || request.ConversationTypeId <= 0)
                return ServiceResult<object>.Fail("اطلاعات ارسالی معتبر نیست", 400);

            var conversation = new Conversation
            {
                Title = request.Title,
                Description = request.Description,
                ConversationTypeId = request.ConversationTypeId,
                CreatedAt = DateTime.Now,
                IsActive = true
            };

            await _conversations.CreateAsync(conversation);
            await _unitOfWork.SaveChangesAsync();

            var response = new ShowConversationResponse
            {
                Id = conversation.Id,
                Title = conversation.Title,
                Description = conversation.Description,
                ConversationTypeId = conversation.ConversationTypeId,
                CreatedBy = conversation.CreatedBy,
                CreatedAt = DateHelpers.ToPersianDate(conversation.CreatedAt),
                LastMessageAt = DateHelpers.ToPersianDate(conversation.LastMessageAt),
                IsActive = conversation.IsActive
            };

            return ServiceResult<object>.Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in AddConversation");

            return ServiceResult<object>.Fail(
                "خطای داخلی سرور رخ داده است", 500, ex.ToString());
        }
    }

    public async Task<ServiceResult<object>> EditAsync(EditConversationRequest request)
    {
        try
        {
            if (request == null || request.Id <= 0)
                return ServiceResult<object>.Fail("اطلاعات ارسالی معتبر نیست", 400);

            var conversation = await _conversations
                .Get(x => x.Id == request.Id)
                .FirstOrDefaultAsync();

            if (conversation == null)
                return ServiceResult<object>.Fail("گفتگو پیدا نشد", 404);

            conversation.Title = request.Title;
            conversation.Description = request.Description;
            conversation.IsActive = request.IsActive;
            conversation.UpdatedAt = DateTime.Now;

            await _conversations.UpdateAsync(conversation);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<object>.Ok(new
            {
                conversation.Id,
                conversation.Title,
                conversation.Description,
                conversation.IsActive,
                UpdatedAt = DateHelpers.ToPersianDate(conversation.UpdatedAt)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in EditConversation");

            return ServiceResult<object>.Fail(
                "خطای داخلی سرور رخ داده است", 500, ex.ToString());
        }
    }

    public async Task<ServiceResult<object>> DeleteAsync(int conversationId)
    {
        try
        {
            if (conversationId <= 0)
                return ServiceResult<object>.Fail("شناسه معتبر نیست", 400);

            var conversation = await _conversations
                .Get(x => x.Id == conversationId)
                .FirstOrDefaultAsync();

            if (conversation == null)
                return ServiceResult<object>.Fail("گفتگو پیدا نشد", 404);

            conversation.IsActive = false;
            conversation.UpdatedAt = DateTime.Now;

            await _conversations.UpdateAsync(conversation);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<object>.Ok("گفتگو با موفقیت غیرفعال شد");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in DeleteConversation");

            return ServiceResult<object>.Fail(
                "خطای داخلی سرور رخ داده است", 500, ex.ToString());
        }
    }

    public async Task<ServiceResult<object>> GetAllAsync(ShowConversationRequest filter,int pageSize,int pageNumber)
    {
        try
        {
              if(pageNumber <= 0 || pageSize <= 0 )
                return ServiceResult<object>.Fail("اطلاعات صفحه‌بندی معتبر نیست", 400);

            var query = _conversations.Get();

            if (filter != null)
            {
                if (filter.Ids?.Any() == true)
                    query = query.Where(x => filter.Ids.Contains(x.Id));

                if (!string.IsNullOrWhiteSpace(filter.Title))
                    query = query.Where(x => x.Title != null && x.Title.Contains(filter.Title));

                if (filter.ConversationTypeId.HasValue)
                    query = query.Where(x =>
                        x.ConversationTypeId == filter.ConversationTypeId.Value);

                if (filter.IsActive.HasValue)
                    query = query.Where(x => x.IsActive == filter.IsActive.Value);

                if (filter.CreatedBy.HasValue)
                    query = query.Where(x => x.CreatedBy == filter.CreatedBy.Value);
            }

            var totalCount = await query.CountAsync();

            var conversations = await query
                .OrderByDescending(x => x.LastMessageAt ?? x.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var response = conversations.Select(x => new ShowConversationResponse
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                ConversationTypeId = x.ConversationTypeId,
                CreatedBy = x.CreatedBy,
                CreatedAt = DateHelpers.ToPersianDate(x.CreatedAt),
                LastMessageAt = DateHelpers.ToPersianDate(x.LastMessageAt),
                IsActive = x.IsActive
            }).ToList();

            return ServiceResult<object>.Ok(new
            {
                TotalCount = totalCount,
                PageSize = pageSize,
                PageNumber = pageNumber,
                Data = response
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetAllConversations");

            return ServiceResult<object>.Fail(
                "خطای داخلی سرور رخ داده است", 500, ex.ToString());
        }
    }

    public async Task<ServiceResult<object>> GetAsync(int conversationId)
    {
        try
        {
            if (conversationId <= 0)
                return ServiceResult<object>.Fail("شناسه معتبر نیست", 400);

            var conversation = await _conversations
                .Get(x => x.Id == conversationId)
                .FirstOrDefaultAsync();

            if (conversation == null)
                return ServiceResult<object>.Fail("گفتگو پیدا نشد", 404);

            var response = new ShowConversationResponse
            {
                Id = conversation.Id,
                Title = conversation.Title,
                Description = conversation.Description,
                ConversationTypeId = conversation.ConversationTypeId,
                CreatedBy = conversation.CreatedBy,
                CreatedAt = DateHelpers.ToPersianDate(conversation.CreatedAt),
                LastMessageAt = DateHelpers.ToPersianDate(conversation.LastMessageAt),
                IsActive = conversation.IsActive
            };

            return ServiceResult<object>.Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetConversation");

            return ServiceResult<object>.Fail(
                "خطای داخلی سرور رخ داده است", 500, ex.ToString());
        }
    }
}
