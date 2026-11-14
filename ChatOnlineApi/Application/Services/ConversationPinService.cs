
using ChatOnlineApi.Application.Contracts;
using ChatOnlineApi.Domain.Base;
using ChatOnlineApi.Domain.Entities;
using ChatOnlineApi.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedLibrary.Common;
using SharedLibrary.Helpers;
using static ChatOnlineApi.Application.DTOs.ConversationPinDtos;

namespace ChatOnlineApi.Application.Services;

public class ConversationPinService(
    IUnitOfWork _unitOfWork,
    IConversationPinRepository _conversationPins,
    ILogger<ConversationPinService> _logger) : IConversationPinService
{
    public async Task<ServiceResult<object>> AddAsync(AddConversationPinRequest request)
    {
        try
        {
            if (request == null ||
                request.ConversationId <= 0 ||
                request.MessageId <= 0 ||
                request.PinnedBy <= 0)
                return ServiceResult<object>.Fail("اطلاعات ارسالی معتبر نیست", 400);

            var exists = await _conversationPins
                .Get(x => x.ConversationId == request.ConversationId &&
                          x.MessageId == request.MessageId)
                .AnyAsync();

            if (exists)
                return ServiceResult<object>.Fail("این پیام قبلاً سنجاق شده است", 400);

            var pin = new ConversationPin
            {
                ConversationId = request.ConversationId,
                MessageId = request.MessageId,
                PinnedBy = request.PinnedBy,
                PinnedAt = DateTime.Now
            };

            await _conversationPins.CreateAsync(pin);
            await _unitOfWork.SaveChangesAsync();

            var response = new ShowConversationPinResponse
            {
                Id = pin.Id,
                ConversationId = pin.ConversationId,
                MessageId = pin.MessageId,
                PinnedBy = pin.PinnedBy,
                PinnedAt = DateHelpers.ToPersianDate(pin.PinnedAt)
            };

            return ServiceResult<object>.Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in AddConversationPin");

            return ServiceResult<object>.Fail(
                "خطای داخلی سرور رخ داده است", 500, ex.ToString());
        }
    }
    public async Task<ServiceResult<object>> DeleteAsync(int pinId)
    {
        try
        {
            if (pinId <= 0)
                return ServiceResult<object>.Fail("شناسه معتبر نیست", 400);

            var pin = await _conversationPins
                .Get(x => x.Id == pinId)
                .FirstOrDefaultAsync();

            if (pin == null)
                return ServiceResult<object>.Fail("پیام سنجاق‌شده پیدا نشد", 404);

            await _conversationPins.RemoveAsync(pin);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<object>.Ok("پیام با موفقیت از حالت سنجاق خارج شد");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in DeleteConversationPin");

            return ServiceResult<object>.Fail(
                "خطای داخلی سرور رخ داده است", 500, ex.ToString());
        }
    }
    public async Task<ServiceResult<object>> GetAllAsync(ShowConversationPinRequest filter,int pageSize,int pageNumber)
    {
        try
        {
            if (pageSize <= 0 || pageNumber <= 0)
                return ServiceResult<object>.Fail("اطلاعات صفحه‌بندی معتبر نیست", 400);

            var query = _conversationPins.Get();

            if (filter != null)
            {
                if (filter.ConversationId.HasValue)
                    query = query.Where(x =>
                        x.ConversationId == filter.ConversationId.Value);

                if (filter.MessageId.HasValue)
                    query = query.Where(x =>
                        x.MessageId == filter.MessageId.Value);

                if (filter.PinnedBy.HasValue)
                    query = query.Where(x =>
                        x.PinnedBy == filter.PinnedBy.Value);
            }

            var totalCount = await query.CountAsync();

            var pins = await query
                .OrderByDescending(x => x.PinnedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var response = pins.Select(x => new ShowConversationPinResponse
            {
                Id = x.Id,
                ConversationId = x.ConversationId,
                MessageId = x.MessageId,
                PinnedBy = x.PinnedBy,
                PinnedAt = DateHelpers.ToPersianDate(x.PinnedAt)
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
            _logger.LogError(ex, "Error in GetAllConversationPins");

            return ServiceResult<object>.Fail(
                "خطای داخلی سرور رخ داده است", 500, ex.ToString());
        }
    }
    public async Task<ServiceResult<object>> GetAsync(int pinId)
    {
        try
        {
            if (pinId <= 0)
                return ServiceResult<object>.Fail("شناسه معتبر نیست", 400);

            var pin = await _conversationPins
                .Get(x => x.Id == pinId)
                .FirstOrDefaultAsync();

            if (pin == null)
                return ServiceResult<object>.Fail("پیام سنجاق‌شده پیدا نشد", 404);

            var response = new ShowConversationPinResponse
            {
                Id = pin.Id,
                ConversationId = pin.ConversationId,
                MessageId = pin.MessageId,
                PinnedBy = pin.PinnedBy,
                PinnedAt = DateHelpers.ToPersianDate(pin.PinnedAt)
            };

            return ServiceResult<object>.Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetConversationPin");

            return ServiceResult<object>.Fail(
                "خطای داخلی سرور رخ داده است", 500, ex.ToString());
        }
    }
}
