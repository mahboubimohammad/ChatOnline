
using ChatOnlineApi.Application.Contracts;
using ChatOnlineApi.Domain.Base;
using ChatOnlineApi.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedLibrary.Common;
using SharedLibrary.Helpers;
using static ChatOnlineApi.Application.DTOs.MessageReactionDtos;

namespace ChatOnlineApi.Application.Services;

public class MessageReactionService(
    IUnitOfWork _unitOfWork,
    IMessageReactionRepository _reactions,
    IMessageRepository _messages,
    IReactionTypeRepository _reactionTypes,
    ILogger<MessageReactionService> _logger) : IMessageReactionService
{
    public async Task<ServiceResult<object>> AddAsync(AddMessageReactionRequest request)
    {
        try
        {
            if (request == null ||
                request.MessageId <= 0 ||
                request.UserId <= 0 ||
                request.ReactionTypeId <= 0)
            {
                return ServiceResult<object>.Fail("اطلاعات واکنش معتبر نیست", 400);
            }

            var messageExists = await _messages
                .Get(x => x.Id == request.MessageId && x.DeletedAt == null)
                .AnyAsync();

            if (!messageExists)
                return ServiceResult<object>.Fail("پیام پیدا نشد", 404);

            var reactionTypeExists = await _reactionTypes
                .Get(x => x.Id == request.ReactionTypeId)
                .AnyAsync();

            if (!reactionTypeExists)
                return ServiceResult<object>.Fail("نوع واکنش پیدا نشد", 404);

            var existingReaction = await _reactions
                .Get(x => x.MessageId == request.MessageId && x.UserId == request.UserId)
                .FirstOrDefaultAsync();

            if (existingReaction != null)
            {
                existingReaction.ReactionTypeId = request.ReactionTypeId;

                await _reactions.UpdateAsync(existingReaction);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult<object>.Ok(new ShowMessageReactionResponse
                {
                    Id = existingReaction.Id,
                    MessageId = existingReaction.MessageId,
                    UserId = existingReaction.UserId,
                    ReactionTypeId = existingReaction.ReactionTypeId,
                    CreatedAt = DateHelpers.ToPersianDate(existingReaction.CreatedAt)
                });
            }

            var reaction = new Domain.Entities.MessageReaction
            {
                MessageId = request.MessageId,
                UserId = request.UserId,
                ReactionTypeId = request.ReactionTypeId,
                CreatedAt = DateTime.Now
            };

            await _reactions.CreateAsync(reaction);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<object>.Ok(new ShowMessageReactionResponse
            {
                Id = reaction.Id,
                MessageId = reaction.MessageId,
                UserId = reaction.UserId,
                ReactionTypeId = reaction.ReactionTypeId,
                CreatedAt = DateHelpers.ToPersianDate(reaction.CreatedAt)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in AddMessageReaction");

            return ServiceResult<object>.Fail(
                "خطای داخلی سرور رخ داده است", 500, ex.ToString());
        }
    }
    public async Task<ServiceResult<object>> DeleteAsync(int reactionId)
    {
        try
        {
            if (reactionId <= 0)
                return ServiceResult<object>.Fail("شناسه واکنش معتبر نیست", 400);

            var reaction = await _reactions
                .Get(x => x.Id == reactionId)
                .FirstOrDefaultAsync();

            if (reaction == null)
                return ServiceResult<object>.Fail("واکنش پیدا نشد", 404);

            await _reactions.RemoveAsync(reaction);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<object>.Ok("واکنش با موفقیت حذف شد");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in DeleteMessageReaction");

            return ServiceResult<object>.Fail(
                "خطای داخلی سرور رخ داده است", 500, ex.ToString());
        }
    }
    public async Task<ServiceResult<object>> GetAllAsync(ShowMessageReactionRequest filter,int pageSize,int pageNumber)
    {
        try
        {
            if (pageSize <= 0 || pageNumber <= 0)
                return ServiceResult<object>.Fail("اطلاعات صفحه‌بندی معتبر نیست", 400);

            var query = _reactions.Get();

            if (filter != null)
            {
                if (filter.Ids?.Any() == true)
                    query = query.Where(x => filter.Ids.Contains(x.Id));

                if (filter.MessageId.HasValue)
                    query = query.Where(x => x.MessageId == filter.MessageId.Value);

                if (filter.UserId.HasValue)
                    query = query.Where(x => x.UserId == filter.UserId.Value);

                if (filter.ReactionTypeId.HasValue)
                    query = query.Where(x => x.ReactionTypeId == filter.ReactionTypeId.Value);
            }

            var totalCount = await query.CountAsync();

            var reactions = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var response = reactions.Select(x => new ShowMessageReactionResponse
            {
                Id = x.Id,
                MessageId = x.MessageId,
                UserId = x.UserId,
                ReactionTypeId = x.ReactionTypeId,
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
            _logger.LogError(ex, "Error in GetAllMessageReactions");

            return ServiceResult<object>.Fail(
                "خطای داخلی سرور رخ داده است", 500, ex.ToString());
        }
    }
    public async Task<ServiceResult<object>> GetAsync(int reactionId)
    {
        try
        {
            if (reactionId <= 0)
                return ServiceResult<object>.Fail("شناسه واکنش معتبر نیست", 400);

            var reaction = await _reactions
                .Get(x => x.Id == reactionId)
                .FirstOrDefaultAsync();

            if (reaction == null)
                return ServiceResult<object>.Fail("واکنش پیدا نشد", 404);

            return ServiceResult<object>.Ok(new ShowMessageReactionResponse
            {
                Id = reaction.Id,
                MessageId = reaction.MessageId,
                UserId = reaction.UserId,
                ReactionTypeId = reaction.ReactionTypeId,
                CreatedAt = DateHelpers.ToPersianDate(reaction.CreatedAt)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetMessageReaction");

            return ServiceResult<object>.Fail(
                "خطای داخلی سرور رخ داده است", 500, ex.ToString());
        }
    }
}
