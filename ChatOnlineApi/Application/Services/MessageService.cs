using ChatOnlineApi.Application.Contracts;
using ChatOnlineApi.Domain.Base;
using ChatOnlineApi.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedLibrary.Common;
using SharedLibrary.Helpers;
using static ChatOnlineApi.Application.DTOs.MessageDtos;

namespace ChatOnlineApi.Application.Services;

public class MessageService(
    IUnitOfWork _unitOfWork,
    IMessageRepository _messages,
    IConversationRepository _conversations,
    ILogger<MessageService> _logger) : IMessageService
{
    public async Task<ServiceResult<object>> AddAsync(AddMessageRequest request)
    {
        try
        {
            if (request == null ||
                request.ConversationId <= 0 ||
                request.SenderId <= 0 ||
                request.MessageTypeId <= 0 ||
                string.IsNullOrWhiteSpace(request.Text))
            {
                return ServiceResult<object>.Fail("اطلاعات پیام معتبر نیست", 400);
            }

            var conversation = await _conversations
                .Get(x => x.Id == request.ConversationId && x.IsActive)
                .FirstOrDefaultAsync();

            if (conversation == null)
                return ServiceResult<object>.Fail("گفتگو پیدا نشد یا غیرفعال است", 404);

            if (request.ReplyToMessageId.HasValue)
            {
                var replyMessageExists = await _messages
                    .Get(x =>
                        x.Id == request.ReplyToMessageId.Value &&
                        x.ConversationId == request.ConversationId &&
                        x.DeletedAt == null)
                    .AnyAsync();

                if (!replyMessageExists)
                {
                    return ServiceResult<object>.Fail(
                        "پیام موردنظر برای پاسخ پیدا نشد", 404);
                }
            }

            var message = new Domain.Entities.Message
            {
                ConversationId = request.ConversationId,
                SenderId = request.SenderId,
                SenderName = request.SenderName?.Trim(),
                Text = request.Text.Trim(),
                MessageTypeId = request.MessageTypeId,
                ReplyToMessageId = request.ReplyToMessageId,
                MessageReactionId = request.MessageReactionId,
                CreatedAt = DateTime.Now
            };

            await _messages.CreateAsync(message);

            conversation.LastMessageAt = message.CreatedAt;
            conversation.UpdatedAt = DateTime.Now;

            await _conversations.UpdateAsync(conversation);
            await _unitOfWork.SaveChangesAsync();

            var savedMessage = await _messages
                .Get(x => x.Id == message.Id)
                .Include(x => x.MessageType)
                .Include(x => x.MessageReaction)
                .FirstOrDefaultAsync();

            if (savedMessage == null)
                return ServiceResult<object>.Fail("پیام ثبت شد اما دریافت اطلاعات آن ناموفق بود", 500);

            var response = new ShowMessageResponse
            {
                Id = savedMessage.Id,
                ConversationId = savedMessage.ConversationId,
                SenderId = savedMessage.SenderId,
                SenderName = savedMessage.SenderName,
                Text = savedMessage.Text,
                CreatedAt = DateHelpers.ToPersianDate(savedMessage.CreatedAt),
                ReplyToMessageId = savedMessage.ReplyToMessageId,
                MessageTypeId = savedMessage.MessageTypeId,
                MessageTypeTitle = savedMessage.MessageType?.Title,
                MessageReactionId = savedMessage.MessageReactionId,
                MessageReactionTitle = savedMessage.MessageReaction?.Title,
                DeliveredAt = DateHelpers.ToPersianDate(savedMessage.DeliveredAt),
                ReadAt = DateHelpers.ToPersianDate(savedMessage.ReadAt),
                DeletedAt = DateHelpers.ToPersianDate(savedMessage.DeletedAt),
                EditedAt = DateHelpers.ToPersianDate(savedMessage.EditedAt)
            };

            return ServiceResult<object>.Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in AddMessage");

            return ServiceResult<object>.Fail(
                "خطای داخلی سرور رخ داده است", 500, ex.ToString());
        }
    }

    public async Task<ServiceResult<object>> EditAsync(EditMessageRequest request)
    {
        try
        {
            if (request == null ||
                request.Id <= 0 ||
                string.IsNullOrWhiteSpace(request.Text))
            {
                return ServiceResult<object>.Fail("اطلاعات پیام معتبر نیست", 400);
            }

            var message = await _messages
                .Get(x => x.Id == request.Id && x.DeletedAt == null)
                .FirstOrDefaultAsync();

            if (message == null)
                return ServiceResult<object>.Fail("پیام پیدا نشد", 404);

            message.Text = request.Text.Trim();
            message.EditedAt = DateTime.Now;

            await _messages.UpdateAsync(message);
            await _unitOfWork.SaveChangesAsync();

            var updatedMessage = await _messages
                .Get(x => x.Id == message.Id)
                .Include(x => x.MessageType)
                .Include(x => x.MessageReaction)
                .FirstOrDefaultAsync();

            if (updatedMessage == null)
                return ServiceResult<object>.Fail("پیام پیدا نشد", 404);

            var response = new ShowMessageResponse
            {
                Id = updatedMessage.Id,
                ConversationId = updatedMessage.ConversationId,
                SenderId = updatedMessage.SenderId,
                SenderName = updatedMessage.SenderName,
                Text = updatedMessage.Text,
                CreatedAt = DateHelpers.ToPersianDate(updatedMessage.CreatedAt),
                ReplyToMessageId = updatedMessage.ReplyToMessageId,
                MessageTypeId = updatedMessage.MessageTypeId,
                MessageTypeTitle = updatedMessage.MessageType?.Title,
                MessageReactionId = updatedMessage.MessageReactionId,
                MessageReactionTitle = updatedMessage.MessageReaction?.Title,
                DeliveredAt = DateHelpers.ToPersianDate(updatedMessage.DeliveredAt),
                ReadAt = DateHelpers.ToPersianDate(updatedMessage.ReadAt),
                DeletedAt = DateHelpers.ToPersianDate(updatedMessage.DeletedAt),
                EditedAt = DateHelpers.ToPersianDate(updatedMessage.EditedAt)
            };

            return ServiceResult<object>.Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in EditMessage");

            return ServiceResult<object>.Fail(
                "خطای داخلی سرور رخ داده است", 500, ex.ToString());
        }
    }

    public async Task<ServiceResult<object>> DeleteAsync(int messageId)
    {
        try
        {
            if (messageId <= 0)
                return ServiceResult<object>.Fail("شناسه پیام معتبر نیست", 400);

            var message = await _messages
                .Get(x => x.Id == messageId && x.DeletedAt == null)
                .FirstOrDefaultAsync();

            if (message == null)
                return ServiceResult<object>.Fail("پیام پیدا نشد", 404);

            message.DeletedAt = DateTime.Now;

            await _messages.UpdateAsync(message);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<object>.Ok("پیام با موفقیت حذف شد");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in DeleteMessage");

            return ServiceResult<object>.Fail(
                "خطای داخلی سرور رخ داده است", 500, ex.ToString());
        }
    }

    public async Task<ServiceResult<object>> GetAllAsync(
        ShowMessageRequest filter,
        int pageSize,
        int pageNumber)
    {
        try
        {
            if (pageSize <= 0 || pageNumber <= 0)
                return ServiceResult<object>.Fail("اطلاعات صفحه‌بندی معتبر نیست", 400);

            var query = _messages.Get(x => x.DeletedAt == null);

            if (filter != null)
            {
                if (filter.Ids?.Any() == true)
                    query = query.Where(x => filter.Ids.Contains(x.Id));

                if (filter.ConversationId.HasValue)
                    query = query.Where(x =>
                        x.ConversationId == filter.ConversationId.Value);

                if (filter.SenderId.HasValue)
                    query = query.Where(x =>
                        x.SenderId == filter.SenderId.Value);

                if (filter.MessageTypeId.HasValue)
                    query = query.Where(x =>
                        x.MessageTypeId == filter.MessageTypeId.Value);

                if (filter.MessageReactionId.HasValue)
                    query = query.Where(x =>
                        x.MessageReactionId == filter.MessageReactionId.Value);

                if (!string.IsNullOrWhiteSpace(filter.Text))
                    query = query.Where(x =>
                        x.Text.Contains(filter.Text));
            }

            var totalCount = await query.CountAsync();

            var messages = await query
                .Include(x => x.MessageType)
                .Include(x => x.MessageReaction)
                .OrderByDescending(x => x.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var response = messages.Select(x => new ShowMessageResponse
            {
                Id = x.Id,
                ConversationId = x.ConversationId,
                SenderId = x.SenderId,
                SenderName = x.SenderName,
                Text = x.Text,
                CreatedAt = DateHelpers.ToPersianDate(x.CreatedAt),
                ReplyToMessageId = x.ReplyToMessageId,
                MessageTypeId = x.MessageTypeId,
                MessageTypeTitle = x.MessageType?.Title,
                MessageReactionId = x.MessageReactionId,
                MessageReactionTitle = x.MessageReaction?.Title,
                DeliveredAt = DateHelpers.ToPersianDate(x.DeliveredAt),
                ReadAt = DateHelpers.ToPersianDate(x.ReadAt),
                DeletedAt = DateHelpers.ToPersianDate(x.DeletedAt),
                EditedAt = DateHelpers.ToPersianDate(x.EditedAt)
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
            _logger.LogError(ex, "Error in GetAllMessages");

            return ServiceResult<object>.Fail(
                "خطای داخلی سرور رخ داده است", 500, ex.ToString());
        }
    }

    public async Task<ServiceResult<object>> GetAsync(int messageId)
    {
        try
        {
            if (messageId <= 0)
                return ServiceResult<object>.Fail("شناسه پیام معتبر نیست", 400);

            var message = await _messages
                .Get(x => x.Id == messageId && x.DeletedAt == null)
                .Include(x => x.MessageType)
                .Include(x => x.MessageReaction)
                .FirstOrDefaultAsync();

            if (message == null)
                return ServiceResult<object>.Fail("پیام پیدا نشد", 404);

            var response = new ShowMessageResponse
            {
                Id = message.Id,
                ConversationId = message.ConversationId,
                SenderId = message.SenderId,
                SenderName = message.SenderName,
                Text = message.Text,
                CreatedAt = DateHelpers.ToPersianDate(message.CreatedAt),
                ReplyToMessageId = message.ReplyToMessageId,
                MessageTypeId = message.MessageTypeId,
                MessageTypeTitle = message.MessageType?.Title,
                MessageReactionId = message.MessageReactionId,
                MessageReactionTitle = message.MessageReaction?.Title,
                DeliveredAt = DateHelpers.ToPersianDate(message.DeliveredAt),
                ReadAt = DateHelpers.ToPersianDate(message.ReadAt),
                DeletedAt = DateHelpers.ToPersianDate(message.DeletedAt),
                EditedAt = DateHelpers.ToPersianDate(message.EditedAt)
            };

            return ServiceResult<object>.Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetMessage");

            return ServiceResult<object>.Fail(
                "خطای داخلی سرور رخ داده است", 500, ex.ToString());
        }
    }
}