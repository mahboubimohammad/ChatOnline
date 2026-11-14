using ChatOnlineApi.Application.Contracts;
using ChatOnlineApi.Domain.Base;
using ChatOnlineApi.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedLibrary.Common;
using SharedLibrary.Helpers;
using static ChatOnlineApi.Application.DTOs.MessageMentionDtos;

namespace ChatOnlineApi.Application.Services;

public class MessageMentionService(
    IUnitOfWork _unitOfWork,
    IMessageMentionRepository _mentions,
    IMessageRepository _messages,
    ILogger<MessageMentionService> _logger) : IMessageMentionService
{
    public async Task<ServiceResult<object>> AddAsync(AddMessageMentionRequest request)
    {
        try
        {
            if (request == null || request.MessageId <= 0 || request.UserId <= 0)
                return ServiceResult<object>.Fail("اطلاعات اشاره به کاربر معتبر نیست", 400);

            var messageExists = await _messages
                .Get(x => x.Id == request.MessageId && x.DeletedAt == null)
                .AnyAsync();

            if (!messageExists)
                return ServiceResult<object>.Fail("پیام پیدا نشد", 404);

            var exists = await _mentions
                .Get(x => x.MessageId == request.MessageId && x.UserId == request.UserId)
                .AnyAsync();

            if (exists)
                return ServiceResult<object>.Fail("این کاربر قبلاً در پیام مشخص شده است", 400);

            var mention = new Domain.Entities.MessageMention
            {
                MessageId = request.MessageId,
                UserId = request.UserId,
                CreatedAt = DateTime.Now
            };

            await _mentions.CreateAsync(mention);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<object>.Ok(new ShowMessageMentionResponse
            {
                Id = mention.Id,
                MessageId = mention.MessageId,
                UserId = mention.UserId,
                CreatedAt = DateHelpers.ToPersianDate(mention.CreatedAt)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in AddMessageMention");
            return ServiceResult<object>.Fail("خطای داخلی سرور رخ داده است", 500, ex.ToString());
        }
    }

    public async Task<ServiceResult<object>> DeleteAsync(int mentionId)
    {
        try
        {
            if (mentionId <= 0)
                return ServiceResult<object>.Fail("شناسه معتبر نیست", 400);

            var mention = await _mentions
                .Get(x => x.Id == mentionId)
                .FirstOrDefaultAsync();

            if (mention == null)
                return ServiceResult<object>.Fail("اشاره به کاربر پیدا نشد", 404);

            await _mentions.RemoveAsync(mention);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<object>.Ok("اشاره به کاربر با موفقیت حذف شد");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in DeleteMessageMention");
            return ServiceResult<object>.Fail("خطای داخلی سرور رخ داده است", 500, ex.ToString());
        }
    }

    public async Task<ServiceResult<object>> GetAllAsync(ShowMessageMentionRequest filter, int pageSize, int pageNumber)
    {
        try
        {
            if (pageSize <= 0 || pageNumber <= 0)
                return ServiceResult<object>.Fail("اطلاعات صفحه‌بندی معتبر نیست", 400);

            var query = _mentions.Get();

            if (filter != null)
            {
                if (filter.Ids?.Any() == true)
                    query = query.Where(x => filter.Ids.Contains(x.Id));

                if (filter.MessageId.HasValue)
                    query = query.Where(x => x.MessageId == filter.MessageId.Value);

                if (filter.UserId.HasValue)
                    query = query.Where(x => x.UserId == filter.UserId.Value);
            }

            var totalCount = await query.CountAsync();

            var mentions = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var response = mentions.Select(x => new ShowMessageMentionResponse
            {
                Id = x.Id,
                MessageId = x.MessageId,
                UserId = x.UserId,
                CreatedAt = DateHelpers.ToPersianDate(x.CreatedAt)
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
            _logger.LogError(ex, "Error in GetAllMessageMentions");
            return ServiceResult<object>.Fail("خطای داخلی سرور رخ داده است", 500, ex.ToString());
        }
    }

    public async Task<ServiceResult<object>> GetAsync(int mentionId)
    {
        try
        {
            if (mentionId <= 0)
                return ServiceResult<object>.Fail("شناسه معتبر نیست", 400);

            var mention = await _mentions
                .Get(x => x.Id == mentionId)
                .FirstOrDefaultAsync();

            if (mention == null)
                return ServiceResult<object>.Fail("اشاره به کاربر پیدا نشد", 404);

            return ServiceResult<object>.Ok(new ShowMessageMentionResponse
            {
                Id = mention.Id,
                MessageId = mention.MessageId,
                UserId = mention.UserId,
                CreatedAt = DateHelpers.ToPersianDate(mention.CreatedAt)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetMessageMention");
            return ServiceResult<object>.Fail("خطای داخلی سرور رخ داده است", 500, ex.ToString());
        }
    }
}
