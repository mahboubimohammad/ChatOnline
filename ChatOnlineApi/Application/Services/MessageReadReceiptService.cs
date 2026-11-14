using ChatOnlineApi.Application.Contracts;
using ChatOnlineApi.Domain.Base;
using ChatOnlineApi.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedLibrary.Common;
using SharedLibrary.Helpers;
using static ChatOnlineApi.Application.DTOs.MessageReadReceiptDtos;

namespace ChatOnlineApi.Application.Services;

public class MessageReadReceiptService(
    IUnitOfWork _unitOfWork,
    IMessageReadReceiptRepository _receipts,
    IMessageRepository _messages,
    ILogger<MessageReadReceiptService> _logger) : IMessageReadReceiptService
{
    public async Task<ServiceResult<object>> AddAsync(AddMessageReadReceiptRequest request)
    {
        try
        {
            if (request == null || request.MessageId <= 0 || request.UserId <= 0)
                return ServiceResult<object>.Fail("اطلاعات رسید پیام معتبر نیست", 400);

            var message = await _messages
                .Get(x => x.Id == request.MessageId && x.DeletedAt == null)
                .FirstOrDefaultAsync();

            if (message == null)
                return ServiceResult<object>.Fail("پیام پیدا نشد", 404);

            var receipt = await _receipts
                .Get(x => x.MessageId == request.MessageId && x.UserId == request.UserId)
                .FirstOrDefaultAsync();

            if (receipt != null)
                return ServiceResult<object>.Fail("رسید این پیام قبلاً ثبت شده است", 400);

            var newReceipt = new Domain.Entities.MessageReadReceipt
            {
                MessageId = request.MessageId,
                UserId = request.UserId,
                DeliveredAt = DateTime.Now
            };

            await _receipts.CreateAsync(newReceipt);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<object>.Ok(new ShowMessageReadReceiptResponse
            {
                Id = newReceipt.Id,
                MessageId = newReceipt.MessageId,
                UserId = newReceipt.UserId,
                DeliveredAt = DateHelpers.ToPersianDate(newReceipt.DeliveredAt),
                ReadAt = DateHelpers.ToPersianDate(newReceipt.ReadAt)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in AddMessageReadReceipt");
            return ServiceResult<object>.Fail("خطای داخلی سرور رخ داده است", 500, ex.ToString());
        }
    }

    public async Task<ServiceResult<object>> EditAsync(EditMessageReadReceiptRequest request)
    {
        try
        {
            if (request == null || request.Id <= 0)
                return ServiceResult<object>.Fail("اطلاعات ارسالی معتبر نیست", 400);

            var receipt = await _receipts
                .Get(x => x.Id == request.Id)
                .FirstOrDefaultAsync();

            if (receipt == null)
                return ServiceResult<object>.Fail("رسید پیام پیدا نشد", 404);

            receipt.ReadAt = request.IsRead ? DateTime.Now : null;

            await _receipts.UpdateAsync(receipt);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<object>.Ok(new ShowMessageReadReceiptResponse
            {
                Id = receipt.Id,
                MessageId = receipt.MessageId,
                UserId = receipt.UserId,
                DeliveredAt = DateHelpers.ToPersianDate(receipt.DeliveredAt),
                ReadAt = DateHelpers.ToPersianDate(receipt.ReadAt)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in EditMessageReadReceipt");
            return ServiceResult<object>.Fail("خطای داخلی سرور رخ داده است", 500, ex.ToString());
        }
    }

    public async Task<ServiceResult<object>> DeleteAsync(int receiptId)
    {
        try
        {
            if (receiptId <= 0)
                return ServiceResult<object>.Fail("شناسه رسید معتبر نیست", 400);

            var receipt = await _receipts
                .Get(x => x.Id == receiptId)
                .FirstOrDefaultAsync();

            if (receipt == null)
                return ServiceResult<object>.Fail("رسید پیام پیدا نشد", 404);

            await _receipts.RemoveAsync(receipt);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<object>.Ok("رسید پیام با موفقیت حذف شد");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in DeleteMessageReadReceipt");
            return ServiceResult<object>.Fail("خطای داخلی سرور رخ داده است", 500, ex.ToString());
        }
    }

    public async Task<ServiceResult<object>> GetAllAsync(
        ShowMessageReadReceiptRequest filter, int pageSize, int pageNumber)
    {
        try
        {
            if (pageSize <= 0 || pageNumber <= 0)
                return ServiceResult<object>.Fail("اطلاعات صفحه‌بندی معتبر نیست", 400);

            var query = _receipts.Get();

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

            var receipts = await query
                .OrderByDescending(x => x.DeliveredAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var response = receipts.Select(x => new ShowMessageReadReceiptResponse
            {
                Id = x.Id,
                MessageId = x.MessageId,
                UserId = x.UserId,
                DeliveredAt = DateHelpers.ToPersianDate(x.DeliveredAt),
                ReadAt = DateHelpers.ToPersianDate(x.ReadAt)
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
            _logger.LogError(ex, "Error in GetAllMessageReadReceipts");
            return ServiceResult<object>.Fail("خطای داخلی سرور رخ داده است", 500, ex.ToString());
        }
    }

    public async Task<ServiceResult<object>> GetAsync(int receiptId)
    {
        try
        {
            if (receiptId <= 0)
                return ServiceResult<object>.Fail("شناسه رسید معتبر نیست", 400);

            var receipt = await _receipts
                .Get(x => x.Id == receiptId)
                .FirstOrDefaultAsync();

            if (receipt == null)
                return ServiceResult<object>.Fail("رسید پیام پیدا نشد", 404);

            return ServiceResult<object>.Ok(new ShowMessageReadReceiptResponse
            {
                Id = receipt.Id,
                MessageId = receipt.MessageId,
                UserId = receipt.UserId,
                DeliveredAt = DateHelpers.ToPersianDate(receipt.DeliveredAt),
                ReadAt = DateHelpers.ToPersianDate(receipt.ReadAt)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetMessageReadReceipt");
            return ServiceResult<object>.Fail("خطای داخلی سرور رخ داده است", 500, ex.ToString());
        }
    }
}
