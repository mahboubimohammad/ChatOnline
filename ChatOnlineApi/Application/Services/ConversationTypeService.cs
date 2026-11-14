
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
    public async Task<ServiceResult<object>> AddAsync(AddConversationTypeRequest request)
    {
        try
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Title))
                return ServiceResult<object>.Fail("عنوان نوع گفتگو معتبر نیست", 400);

            var title = request.Title.Trim();

            var exists = await _conversationTypes
                .Get(x => x.Title == title)
                .AnyAsync();

            if (exists)
                return ServiceResult<object>.Fail("این نوع گفتگو قبلاً ثبت شده است", 400);

            var conversationType = new ConversationType
            {
                Title = title
            };

            await _conversationTypes.CreateAsync(conversationType);
            await _unitOfWork.SaveChangesAsync();

            var response = new ShowConversationTypeResponse
            {
                Id = conversationType.Id,
                Title = conversationType.Title
            };

            return ServiceResult<object>.Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in AddConversationType");

            return ServiceResult<object>.Fail(
                "خطای داخلی سرور رخ داده است", 500, ex.ToString());
        }
    }

    public async Task<ServiceResult<object>> EditAsync(EditConversationTypeRequest request)
    {
        try
        {
            if (request == null ||
                request.Id <= 0 ||
                string.IsNullOrWhiteSpace(request.Title))
                return ServiceResult<object>.Fail("اطلاعات ارسالی معتبر نیست", 400);

            var conversationType = await _conversationTypes
                .Get(x => x.Id == request.Id)
                .FirstOrDefaultAsync();

            if (conversationType == null)
                return ServiceResult<object>.Fail("نوع گفتگو پیدا نشد", 404);

            var title = request.Title.Trim();

            var exists = await _conversationTypes
                .Get(x => x.Id != request.Id && x.Title == title)
                .AnyAsync();

            if (exists)
                return ServiceResult<object>.Fail("این عنوان قبلاً ثبت شده است", 400);

            conversationType.Title = title;

            await _conversationTypes.UpdateAsync(conversationType);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<object>.Ok(new ShowConversationTypeResponse
            {
                Id = conversationType.Id,
                Title = conversationType.Title
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in EditConversationType");

            return ServiceResult<object>.Fail(
                "خطای داخلی سرور رخ داده است", 500, ex.ToString());
        }
    }

    public async Task<ServiceResult<object>> DeleteAsync(int conversationTypeId)
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

            var hasConversation = await _conversations
                .Get(x => x.ConversationTypeId == conversationTypeId)
                .AnyAsync();

            if (hasConversation)
                return ServiceResult<object>.Fail(
                    "این نوع گفتگو در گفتگوهای ثبت‌شده استفاده شده و قابل حذف نیست", 400);

            await _conversationTypes.RemoveAsync(conversationType);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<object>.Ok("نوع گفتگو با موفقیت حذف شد");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in DeleteConversationType");

            return ServiceResult<object>.Fail(
                "خطای داخلی سرور رخ داده است", 500, ex.ToString());
        }
    }

    public async Task<ServiceResult<object>> GetAllAsync(ShowConversationTypeRequest filter,int pageSize,int pageNumber)
    {
        try
        {
            if (pageSize <= 0 || pageNumber <= 0)
                return ServiceResult<object>.Fail("اطلاعات صفحه‌بندی معتبر نیست", 400);

            var query = _conversationTypes.Get();

            if (filter != null)
            {
                if (filter.Ids?.Any() == true)
                    query = query.Where(x => filter.Ids.Contains(x.Id));

                if (!string.IsNullOrWhiteSpace(filter.Title))
                    query = query.Where(x => x.Title.Contains(filter.Title));
            }

            var totalCount = await query.CountAsync();

            var conversationTypes = await query
                .OrderBy(x => x.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var response = conversationTypes.Select(x =>
                new ShowConversationTypeResponse
                {
                    Id = x.Id,
                    Title = x.Title
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
            _logger.LogError(ex, "Error in GetAllConversationTypes");

            return ServiceResult<object>.Fail(
                "خطای داخلی سرور رخ داده است", 500, ex.ToString());
        }
    }

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
