
using ChatOnlineApi.Application.Contracts;
using ChatOnlineApi.Domain.Base;
using ChatOnlineApi.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedLibrary.Common;
using SharedLibrary.Helpers;
using static ChatOnlineApi.Application.DTOs.MessageAttachmentDtos;

namespace ChatOnlineApi.Application.Services;

public class MessageAttachmentService(
    IUnitOfWork _unitOfWork,
    IMessageAttachmentRepository _attachments,
    IMessageRepository _messages,
    ILogger<MessageAttachmentService> _logger) : IMessageAttachmentService
{
    public async Task<ServiceResult<object>> AddAsync(AddMessageAttachmentRequest request)
    {
        try
        {
            if (request == null ||
                request.MessageId <= 0 ||
                string.IsNullOrWhiteSpace(request.FileUrl) ||
                string.IsNullOrWhiteSpace(request.FileName) ||
                request.FileSize <= 0 ||
                string.IsNullOrWhiteSpace(request.ContentType))
            {
                return ServiceResult<object>.Fail("اطلاعات فایل پیوست معتبر نیست", 400);
            }

            var messageExists = await _messages
                .Get(x => x.Id == request.MessageId && x.DeletedAt == null)
                .AnyAsync();

            if (!messageExists)
                return ServiceResult<object>.Fail("پیام موردنظر پیدا نشد", 404);

            var attachment = new Domain.Entities.MessageAttachment
            {
                MessageId = request.MessageId,
                FileUrl = request.FileUrl.Trim(),
                FileName = request.FileName.Trim(),
                FileSize = request.FileSize,
                ContentType = request.ContentType.Trim(),
                CreatedAt = DateTime.Now
            };

            await _attachments.CreateAsync(attachment);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<object>.Ok(new ShowMessageAttachmentResponse
            {
                Id = attachment.Id,
                MessageId = attachment.MessageId,
                FileUrl = attachment.FileUrl,
                FileName = attachment.FileName,
                FileSize = attachment.FileSize,
                ContentType = attachment.ContentType,
                CreatedAt = DateHelpers.ToPersianDate(attachment.CreatedAt)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in AddMessageAttachment");

            return ServiceResult<object>.Fail(
                "خطای داخلی سرور رخ داده است", 500, ex.ToString());
        }
    }

    public async Task<ServiceResult<object>> EditAsync(EditMessageAttachmentRequest request)
    {
        try
        {
            if (request == null ||
                request.Id <= 0 ||
                string.IsNullOrWhiteSpace(request.FileName))
            {
                return ServiceResult<object>.Fail("اطلاعات ارسالی معتبر نیست", 400);
            }

            var attachment = await _attachments
                .Get(x => x.Id == request.Id)
                .FirstOrDefaultAsync();

            if (attachment == null)
                return ServiceResult<object>.Fail("فایل پیوست پیدا نشد", 404);

            attachment.FileName = request.FileName.Trim();

            await _attachments.UpdateAsync(attachment);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<object>.Ok(new ShowMessageAttachmentResponse
            {
                Id = attachment.Id,
                MessageId = attachment.MessageId,
                FileUrl = attachment.FileUrl,
                FileName = attachment.FileName,
                FileSize = attachment.FileSize,
                ContentType = attachment.ContentType,
                CreatedAt = DateHelpers.ToPersianDate(attachment.CreatedAt)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in EditMessageAttachment");

            return ServiceResult<object>.Fail(
                "خطای داخلی سرور رخ داده است", 500, ex.ToString());
        }
    }

    public async Task<ServiceResult<object>> DeleteAsync(int attachmentId)
    {
        try
        {
            if (attachmentId <= 0)
                return ServiceResult<object>.Fail("شناسه فایل پیوست معتبر نیست", 400);

            var attachment = await _attachments
                .Get(x => x.Id == attachmentId)
                .FirstOrDefaultAsync();

            if (attachment == null)
                return ServiceResult<object>.Fail("فایل پیوست پیدا نشد", 404);

            await _attachments.RemoveAsync(attachment);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<object>.Ok("فایل پیوست با موفقیت حذف شد");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in DeleteMessageAttachment");

            return ServiceResult<object>.Fail(
                "خطای داخلی سرور رخ داده است", 500, ex.ToString());
        }
    }

    public async Task<ServiceResult<object>> GetAllAsync(
        ShowMessageAttachmentRequest filter,
        int pageSize,
        int pageNumber)
    {
        try
        {
            if (pageSize <= 0 || pageNumber <= 0)
                return ServiceResult<object>.Fail("اطلاعات صفحه‌بندی معتبر نیست", 400);

            var query = _attachments.Get();

            if (filter != null)
            {
                if (filter.Ids?.Any() == true)
                    query = query.Where(x => filter.Ids.Contains(x.Id));

                if (filter.MessageId.HasValue)
                    query = query.Where(x => x.MessageId == filter.MessageId.Value);

                if (!string.IsNullOrWhiteSpace(filter.FileName))
                    query = query.Where(x => x.FileName.Contains(filter.FileName));

                if (!string.IsNullOrWhiteSpace(filter.ContentType))
                    query = query.Where(x => x.ContentType == filter.ContentType);
            }

            var totalCount = await query.CountAsync();

            var attachments = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var response = attachments.Select(x => new ShowMessageAttachmentResponse
            {
                Id = x.Id,
                MessageId = x.MessageId,
                FileUrl = x.FileUrl,
                FileName = x.FileName,
                FileSize = x.FileSize,
                ContentType = x.ContentType,
                CreatedAt = DateHelpers.ToPersianDate(x.CreatedAt)
            }).ToList();

            return ServiceResult<object>.Ok(new{TotalCount = totalCount,PageSize = pageSize,PageNumber = pageNumber,Data = response});
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetAllMessageAttachments");

            return ServiceResult<object>.Fail(
                "خطای داخلی سرور رخ داده است", 500, ex.ToString());
        }
    }

    public async Task<ServiceResult<object>> GetAsync(int attachmentId)
    {
        try
        {
            if (attachmentId <= 0)
                return ServiceResult<object>.Fail("شناسه فایل پیوست معتبر نیست", 400);

            var attachment = await _attachments
                .Get(x => x.Id == attachmentId)
                .FirstOrDefaultAsync();

            if (attachment == null)
                return ServiceResult<object>.Fail("فایل پیوست پیدا نشد", 404);

            var response = new ShowMessageAttachmentResponse
            {
                Id = attachment.Id,
                MessageId = attachment.MessageId,
                FileUrl = attachment.FileUrl,
                FileName = attachment.FileName,
                FileSize = attachment.FileSize,
                ContentType = attachment.ContentType,
                CreatedAt = DateHelpers.ToPersianDate(attachment.CreatedAt)
            };

            return ServiceResult<object>.Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetMessageAttachment");

            return ServiceResult<object>.Fail(
                "خطای داخلی سرور رخ داده است", 500, ex.ToString());
        }
    }
}
