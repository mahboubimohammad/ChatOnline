
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
    ISupportAgentRepository _supportAgents,
    ILogger<ConversationService> _logger) : IConversationService
{
    public async Task<ServiceResult<object>> AddAsync(AddConversationRequest request)
    {
        try
        {
            if (request == null ||
                !request.UserId.HasValue || request.UserId.Value <= 0)
            {
                return ServiceResult<object>.Fail("اطلاعات ارسالی معتبر نیست", 400);
            }

            var conversation = new Conversation
            {
                UserId = request.UserId,
                SupportAgentId = null,
                SupportAgentName = null,
                CreatedByName = request.CreatedByName?.Trim(),
                Title = request.Title?.Trim(),
                Description = request.Description?.Trim(),
                CreatedAt = DateTime.Now,
                IsActive = true
            };

            await _conversations.CreateAsync(conversation);
            await _unitOfWork.SaveChangesAsync();

            var response = new ShowConversationResponse
            {
                Id = conversation.Id,
                UserId = conversation.UserId,
                SupportAgentId = conversation.SupportAgentId,
                SupportAgentName = conversation.SupportAgentName,
                CreatedByName = conversation.CreatedByName,
                Title = conversation.Title,
                Description = conversation.Description,
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

    public async Task<ServiceResult<object>> AssignSupportAgentAsync(int conversationId, int supportAgentId)
    {
        try
        {
            if (conversationId <= 0 || supportAgentId <= 0)
                return ServiceResult<object>.Fail("شناسه معتبر نیست", 400);

            var supportAgent = await _supportAgents
                .Get(x => x.Id == supportAgentId && x.IsActive)
                .FirstOrDefaultAsync();

            if (supportAgent == null)
                return ServiceResult<object>.Fail(
                    "پشتیبان پیدا نشد ", 404);

            var affectedRows = await _conversations
                .Get(x =>
                    x.Id == conversationId &&
                    x.IsActive &&
                    x.SupportAgentId == null)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.SupportAgentId, supportAgent.Id)
                    .SetProperty(x => x.SupportAgentName, supportAgent.FullName)
                    .SetProperty(x => x.UpdatedAt, DateTime.Now));

            if (affectedRows == 0)
            {
                var conversation = await _conversations
                    .Get(x => x.Id == conversationId)
                    .FirstOrDefaultAsync();

                if (conversation == null)
                    return ServiceResult<object>.Fail("گفتگو پیدا نشد", 404);

                if (!conversation.IsActive)
                    return ServiceResult<object>.Fail(
                        "گفتگو غیرفعال است", 400);

                return ServiceResult<object>.Fail(
                    "این گفتگو قبلاً به یک پشتیبان اختصاص داده شده است", 409);
            }

            return ServiceResult<object>.Ok(new
            {
                ConversationId = conversationId,
                SupportAgentId = supportAgent.Id,
                SupportAgentName = supportAgent.FullName,
                Message = "گفتگو با موفقیت به پشتیبان اختصاص داده شد"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in AssignSupportAgent");

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

            conversation.Title = request.Title?.Trim();
            conversation.Description = request.Description?.Trim();
            conversation.IsActive = request.IsActive;
            conversation.UpdatedAt = DateTime.Now;

            await _conversations.UpdateAsync(conversation);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<object>.Ok(new
            {
                conversation.Id,
                conversation.UserId,
                conversation.SupportAgentId,
                conversation.SupportAgentName,
                conversation.CreatedByName,
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

    public async Task<ServiceResult<object>> GetAllAsync(ShowConversationRequest filter, int pageSize, int pageNumber)
    {
        try
        {
            if (pageSize <= 0 || pageNumber <= 0)
                return ServiceResult<object>.Fail("اطلاعات صفحه‌بندی معتبر نیست", 400);

            var query = _conversations.Get();

            if (filter != null)
            {
                if (filter.Ids?.Any() == true)
                    query = query.Where(x => filter.Ids.Contains(x.Id));

                if (filter.UserId.HasValue)
                    query = query.Where(x => x.UserId == filter.UserId.Value);

                if (filter.SupportAgentId.HasValue)
                    query = query.Where(x => x.SupportAgentId == filter.SupportAgentId.Value);

                if (!string.IsNullOrWhiteSpace(filter.Title))
                    query = query.Where(x =>
                        x.Title != null && x.Title.Contains(filter.Title));

                if (filter.IsActive.HasValue)
                    query = query.Where(x => x.IsActive == filter.IsActive.Value);
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
                UserId = x.UserId,
                SupportAgentId = x.SupportAgentId,
                SupportAgentName = x.SupportAgentName,
                CreatedByName = x.CreatedByName,
                Title = x.Title,
                Description = x.Description,
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
                UserId = conversation.UserId,
                SupportAgentId = conversation.SupportAgentId,
                SupportAgentName = conversation.SupportAgentName,
                CreatedByName = conversation.CreatedByName,
                Title = conversation.Title,
                Description = conversation.Description,
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