using ChatOnlineApi.Application.Contracts;
using ChatOnlineApi.Domain.Base;
using ChatOnlineApi.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
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
    IConversationParticipantRepository _participants,
    IHttpContextAccessor _httpContextAccessor,
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

            var conversationExists = await _conversations
                .Get(x => x.Id == request.ConversationId)
                .AnyAsync();

            if (!conversationExists)
                return ServiceResult<object>.Fail("گفتگو پیدا نشد", 404);

            var message = new Domain.Entities.Message
            {
                ConversationId = request.ConversationId,
                SenderId = request.SenderId,
                Text = request.Text.Trim(),
                MessageTypeId = request.MessageTypeId,
                ReplyToMessageId = request.ReplyToMessageId,
                CreatedAt = DateTime.Now
            };

            if (request.ReplyToMessageId.HasValue)
            {
                var replyMessageExists = await _messages
                    .Get(x =>
                        x.Id == request.ReplyToMessageId.Value &&
                        x.ConversationId == request.ConversationId &&
                        x.DeletedAt == null)
                    .AnyAsync();

                if (!replyMessageExists)
                    return ServiceResult<object>.Fail(
                        "پیام موردنظر برای پاسخ پیدا نشد", 404);
            }

            await _messages.CreateAsync(message);
            await _unitOfWork.SaveChangesAsync();

            var savedMessage = await _messages
                .Get(x => x.Id == message.Id)
                .Include(x => x.MessageType)
                .FirstOrDefaultAsync();

            return ServiceResult<object>.Ok(new ShowMessageResponse
            {
                Id = savedMessage!.Id,
                ConversationId = savedMessage.ConversationId,
                SenderId = savedMessage.SenderId,
                Text = savedMessage.Text,
                CreatedAt = DateHelpers.ToPersianDate(savedMessage.CreatedAt),
                ReplyToMessageId = savedMessage.ReplyToMessageId,
                MessageTypeId = savedMessage.MessageTypeId,
                MessageTypeTitle = savedMessage.MessageType?.Title,
                DeletedAt = DateHelpers.ToPersianDate(savedMessage.DeletedAt),
                EditedAt = DateHelpers.ToPersianDate(savedMessage.EditedAt)
            });
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
            if (request == null || request.Id <= 0 ||
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
                .FirstOrDefaultAsync();

            return ServiceResult<object>.Ok(new ShowMessageResponse
            {
                Id = updatedMessage!.Id,
                ConversationId = updatedMessage.ConversationId,
                SenderId = updatedMessage.SenderId,
                Text = updatedMessage.Text,
                CreatedAt = DateHelpers.ToPersianDate(updatedMessage.CreatedAt),
                ReplyToMessageId = updatedMessage.ReplyToMessageId,
                MessageTypeId = updatedMessage.MessageTypeId,
                MessageTypeTitle = updatedMessage.MessageType?.Title,
                DeletedAt = DateHelpers.ToPersianDate(updatedMessage.DeletedAt),
                EditedAt = DateHelpers.ToPersianDate(updatedMessage.EditedAt)
            });
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

    public async Task<ServiceResult<object>> GetAllAsync(ShowMessageRequest filter, int pageSize, int pageNumber)
    {
        try
        {
            if (pageSize <= 0 || pageNumber <= 0)
                return ServiceResult<object>.Fail(
                    "اطلاعات صفحه‌بندی معتبر نیست", 400);

            var encryptedToken = _httpContextAccessor.HttpContext?
                .User.FindFirst("Token")?.Value;

            var user = SecurityHelpers.GetUserFromToken(encryptedToken);

            if (user == null)
                return ServiceResult<object>.Fail("کاربر معتبر نیست", 401);

            var userId = user.Id;

            var query = _messages.Get(x => x.DeletedAt == null);

            if (filter != null)
            {
                if (filter.Ids?.Any() == true)
                    query = query.Where(x => filter.Ids.Contains(x.Id));

                if (filter.ConversationId.HasValue)
                    query = query.Where(x =>
                        x.ConversationId == filter.ConversationId.Value);

                if (filter.SenderId.HasValue)
                    query = query.Where(x => x.SenderId == filter.SenderId.Value);

                if (filter.MessageTypeId.HasValue)
                    query = query.Where(x =>
                        x.MessageTypeId == filter.MessageTypeId.Value);

                if (!string.IsNullOrWhiteSpace(filter.Text))
                    query = query.Where(x => x.Text.Contains(filter.Text));
            }

            // بررسی عضویت و ثبت خوانده شدن پیام‌ها
            if (filter?.ConversationId > 0)
            {
                var conversationId = filter.ConversationId.Value;

                var isParticipant = await _participants.Get(x =>
                    x.ConversationId == conversationId &&
                    x.UserId == userId &&
                    x.IsActive &&
                    x.LeftAt == null
                ).AnyAsync();

                if (!isParticipant)
                    return ServiceResult<object>.Fail(
                        "شما عضو فعال این گفتگو نیستید", 403);
            }

            var totalCount = await query.CountAsync();

            var messages = await query
                .Include(x => x.MessageType)
                .Include(x => x.MessageReadReceipts
                    .Where(r => r.UserId == userId))
                .OrderByDescending(x => x.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            if (filter?.ConversationId > 0)
            {
                var now = DateTime.Now;

                foreach (var message in messages)
                {
                    if (message.SenderId == userId)
                        continue;

                    var receipt = message.MessageReadReceipts
                        .FirstOrDefault(x => x.UserId == userId);

                    if (receipt == null)
                    {
                        message.MessageReadReceipts.Add(
                            new Domain.Entities.MessageReadReceipt
                            {
                                MessageId = message.Id,
                                UserId = userId,
                                DeliveredAt = now,
                                ReadAt = now
                            });
                    }
                    else if (receipt.ReadAt == null)
                    {
                        receipt.DeliveredAt ??= now;
                        receipt.ReadAt = now;
                    }
                }

                await _unitOfWork.SaveChangesAsync();
            }

            var response = messages.Select(x => new ShowMessageResponse
            {
                Id = x.Id,
                ConversationId = x.ConversationId,
                SenderId = x.SenderId,
                Text = x.Text,
                CreatedAt = DateHelpers.ToPersianDate(x.CreatedAt),
                ReplyToMessageId = x.ReplyToMessageId,
                MessageTypeId = x.MessageTypeId,
                MessageTypeTitle = x.MessageType?.Title,
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
                .FirstOrDefaultAsync();

            if (message == null)
                return ServiceResult<object>.Fail("پیام پیدا نشد", 404);

            var response = new ShowMessageResponse
            {
                Id = message.Id,
                ConversationId = message.ConversationId,
                SenderId = message.SenderId,
                Text = message.Text,
                CreatedAt = DateHelpers.ToPersianDate(message.CreatedAt),
                ReplyToMessageId = message.ReplyToMessageId,
                MessageTypeId = message.MessageTypeId,
                MessageTypeTitle = message.MessageType?.Title,
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