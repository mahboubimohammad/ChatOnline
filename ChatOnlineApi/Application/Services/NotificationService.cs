using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using SharedLibrary.Common;
using SharedLibrary.Helpers;
using ChatOnlineApi.Application.Contracts;
using ChatOnlineApi.Domain.Base;
using ChatOnlineApi.Domain.Interfaces;
using ChatOnlineApi.Infrastructure.Configuration;
using static ChatOnlineApi.Application.DTOs.NotificationDtos;

namespace ChatOnlineApi.Application.Services;

public class NotificationService(
    INotificationRepository _notifications,
    IUnitOfWork _unitOfWork,
    IStringLocalizer<NotificationService> _localizer,
    IOptions<LogOptions> logOpt) : INotificationService
{
    private readonly LogOptions _logOpt = logOpt.Value;

    public async Task<ServiceResult<object>> AddAsync(AddNotificationRequest request)
    {
        try
        {
            if (request.UserId <= 0)
                return ServiceResult<object>.Fail(_localizer["InvalidUserId"], 400);

            if (string.IsNullOrWhiteSpace(request.Title))
                return ServiceResult<object>.Fail(_localizer["TitleRequired"], 400);

            if (string.IsNullOrWhiteSpace(request.NotificationType))
                return ServiceResult<object>.Fail(_localizer["NotificationTypeRequired"], 400);

            var notification = new Domain.Entities.Notification
            {
                UserId = request.UserId,
                ConversationId = request.ConversationId,
                MessageId = request.MessageId,
                Title = request.Title.Trim(),
                Body = request.Body?.Trim(),
                NotificationType = request.NotificationType.Trim(),
                IsRead = false,
                CreatedAt = DateTime.Now
            };

            await _notifications.CreateAsync(notification);
            await _unitOfWork.SaveChangesAsync();

            var response = new ShowNotificationResponse
            {
                Id = notification.Id,
                UserId = notification.UserId,
                ConversationId = notification.ConversationId,
                MessageId = notification.MessageId,
                Title = notification.Title,
                Body = notification.Body,
                NotificationType = notification.NotificationType,
                IsRead = notification.IsRead,
                CreatedAt = DateHelpers.ToPersianDate(notification.CreatedAt),
                ReadAt = DateHelpers.ToPersianDate(notification.ReadAt)
            };

            return ServiceResult<object>.Ok(
                response,
                _localizer["NotificationAddedSuccessfully"]);
        }
        catch (Exception ex)
        {
            SecurityHelpers.InsertToLog(
                _logOpt.InsertLog,
                "AddNotificationAsync - " + ex);

            return ServiceResult<object>.Fail(
                _localizer["InternalServerError"], 500, ex.ToString());
        }
    }

    public async Task<ServiceResult<object>> GetAllAsync(
        ShowNotificationRequest request,
        int pageSize,
        int pageNumber)
    {
        try
        {
            var query = _notifications.Get();

            if (request.Ids != null && request.Ids.Any())
                query = query.Where(x => request.Ids.Contains(x.Id));

            if (request.UserId.HasValue)
                query = query.Where(x => x.UserId == request.UserId.Value);

            if (request.ConversationId.HasValue)
                query = query.Where(x => x.ConversationId == request.ConversationId.Value);

            if (request.MessageId.HasValue)
                query = query.Where(x => x.MessageId == request.MessageId.Value);

            if (request.IsRead.HasValue)
                query = query.Where(x => x.IsRead == request.IsRead.Value);

            if (!string.IsNullOrWhiteSpace(request.NotificationType))
                query = query.Where(x => x.NotificationType.Contains(request.NotificationType.Trim()));

            if (!string.IsNullOrWhiteSpace(request.Title))
                query = query.Where(x => x.Title.Contains(request.Title.Trim()));

            var totalCount = await query.CountAsync();

            pageSize = pageSize <= 0 ? 10 : pageSize;
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;

            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new ShowNotificationResponse
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    ConversationId = x.ConversationId,
                    MessageId = x.MessageId,
                    Title = x.Title,
                    Body = x.Body,
                    NotificationType = x.NotificationType,
                    IsRead = x.IsRead,
                    CreatedAt = DateHelpers.ToPersianDate(x.CreatedAt),
                    ReadAt = DateHelpers.ToPersianDate(x.ReadAt)
                })
                .ToListAsync();

            return ServiceResult<object>.Ok(new
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            });
        }
        catch (Exception ex)
        {
            SecurityHelpers.InsertToLog(
                _logOpt.InsertLog,
                "GetAllNotificationsAsync - " + ex);

            return ServiceResult<object>.Fail(
                _localizer["InternalServerError"], 500, ex.ToString());
        }
    }

    public async Task<ServiceResult<object>> GetAsync(int id)
    {
        try
        {
            if (id <= 0)
                return ServiceResult<object>.Fail(_localizer["InvalidId"], 400);

            var notification = await _notifications.GetByIdAsync(id);

            if (notification == null)
                return ServiceResult<object>.Fail(_localizer["NotificationNotFound"], 404);

            var response = new ShowNotificationResponse
            {
                Id = notification.Id,
                UserId = notification.UserId,
                ConversationId = notification.ConversationId,
                MessageId = notification.MessageId,
                Title = notification.Title,
                Body = notification.Body,
                NotificationType = notification.NotificationType,
                IsRead = notification.IsRead,
                CreatedAt = DateHelpers.ToPersianDate(notification.CreatedAt),
                ReadAt = DateHelpers.ToPersianDate(notification.ReadAt)
            };

            return ServiceResult<object>.Ok(response);
        }
        catch (Exception ex)
        {
            SecurityHelpers.InsertToLog(
                _logOpt.InsertLog,
                "GetNotificationAsync - " + ex);

            return ServiceResult<object>.Fail(
                _localizer["InternalServerError"], 500, ex.ToString());
        }
    }

    public async Task<ServiceResult<object>> MarkAsReadAsync(int id)
    {
        try
        {
            if (id <= 0)
                return ServiceResult<object>.Fail(_localizer["InvalidId"], 400);

            var notification = await _notifications.GetByIdAsync(id);

            if (notification == null)
                return ServiceResult<object>.Fail(_localizer["NotificationNotFound"], 404);

            if (!notification.IsRead)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.Now;

                await _notifications.UpdateAsync(notification);
                await _unitOfWork.SaveChangesAsync();
            }

            return ServiceResult<object>.Ok(
                _localizer["NotificationMarkedAsRead"]);
        }
        catch (Exception ex)
        {
            SecurityHelpers.InsertToLog(
                _logOpt.InsertLog,
                "MarkNotificationAsReadAsync - " + ex);

            return ServiceResult<object>.Fail(
                _localizer["InternalServerError"], 500, ex.ToString());
        }
    }

    public async Task<ServiceResult<object>> DeleteAsync(int id)
    {
        try
        {
            if (id <= 0)
                return ServiceResult<object>.Fail(_localizer["InvalidId"], 400);

            var notification = await _notifications.GetByIdAsync(id);

            if (notification == null)
                return ServiceResult<object>.Fail(_localizer["NotificationNotFound"], 404);

            await _notifications.RemoveAsync(notification);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<object>.Ok(
                _localizer["NotificationDeletedSuccessfully"]);
        }
        catch (Exception ex)
        {
            SecurityHelpers.InsertToLog(
                _logOpt.InsertLog,
                "DeleteNotificationAsync - " + ex);

            return ServiceResult<object>.Fail(
                _localizer["InternalServerError"], 500, ex.ToString());
        }
    }
}