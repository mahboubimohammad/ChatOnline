using ChatOnlineApi.Application.Contracts;
using ChatOnlineApi.Domain.Base;
using ChatOnlineApi.Domain.Interfaces;
using ChatOnlineApi.Infrastructure.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using SharedLibrary.Common;
using SharedLibrary.Helpers;
using static ChatOnlineApi.Application.DTOs.UserPresenceDtos;

namespace ChatOnlineApi.Application.Services;

public class UserPresenceService(
    IUserPresenceRepository _presences,
    IUnitOfWork _unitOfWork,
    IStringLocalizer<UserPresenceService> _localizer,
    IOptions<LogOptions> logOpt) : IUserPresenceService
{
    private readonly LogOptions _logOpt = logOpt.Value;

    public async Task<ServiceResult<object>> GetAllAsync(ShowUserPresenceRequest request,int pageSize,int pageNumber)
    {
        try
        {
            var query = _presences.Get();

            if (request.UserIds != null && request.UserIds.Any())
                query = query.Where(x => request.UserIds.Contains(x.UserId));

            if (request.IsOnline.HasValue)
                query = query.Where(x => x.IsOnline == request.IsOnline.Value);

            var totalCount = await query.CountAsync();

            pageSize = pageSize <= 0 ? 10 : pageSize;
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;

            var presences = await query
                .OrderBy(x => x.UserId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var items = presences.Select(x => new ShowUserPresenceResponse
            {
                Id = x.Id,
                UserId = x.UserId,
                IsOnline = x.IsOnline,
                LastSeenAt = DateHelpers.ToPersianDate(x.LastSeenAt),
                UpdatedAt = DateHelpers.ToPersianDate(x.UpdatedAt)
            }).ToList();

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
                "GetAllUserPresencesAsync - " + ex);

            return ServiceResult<object>.Fail(
                _localizer["InternalServerError"], 500, ex.ToString());
        }
    }

    public async Task<ServiceResult<object>> GetAsync(int userId)
    {
        try
        {
            if (userId <= 0)
                return ServiceResult<object>.Fail(_localizer["InvalidUserId"], 400);

            var presence = await _presences
                .Get(x => x.UserId == userId)
                .FirstOrDefaultAsync();

            if (presence == null)
                return ServiceResult<object>.Fail(_localizer["UserPresenceNotFound"], 404);

            var response = new ShowUserPresenceResponse
            {
                Id = presence.Id,
                UserId = presence.UserId,
                IsOnline = presence.IsOnline,
                LastSeenAt = DateHelpers.ToPersianDate(presence.LastSeenAt),
                UpdatedAt = DateHelpers.ToPersianDate(presence.UpdatedAt)
            };

            return ServiceResult<object>.Ok(response);
        }
        catch (Exception ex)
        {
            SecurityHelpers.InsertToLog(
                _logOpt.InsertLog,
                "GetUserPresenceAsync - " + ex);

            return ServiceResult<object>.Fail(
                _localizer["InternalServerError"], 500, ex.ToString());
        }
    }

    public async Task<ServiceResult<object>> UpdatePresenceAsync(UpdateUserPresenceRequest request)
    {
        try
        {
            if (request.UserId <= 0)
                return ServiceResult<object>.Fail(_localizer["InvalidUserId"], 400);

            var presence = await _presences
                .Get(x => x.UserId == request.UserId)
                .FirstOrDefaultAsync();

            if (presence == null)
            {
                presence = new Domain.Entities.UserPresence
                {
                    UserId = request.UserId,
                    IsOnline = request.IsOnline,
                    LastSeenAt = request.IsOnline ? null : DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                await _presences.CreateAsync(presence);
            }
            else
            {
                presence.IsOnline = request.IsOnline;
                presence.UpdatedAt = DateTime.Now;

                if (!request.IsOnline)
                    presence.LastSeenAt = DateTime.Now;

                await _presences.UpdateAsync(presence);
            }

            await _unitOfWork.SaveChangesAsync();

            var response = new ShowUserPresenceResponse
            {
                Id = presence.Id,
                UserId = presence.UserId,
                IsOnline = presence.IsOnline,
                LastSeenAt = DateHelpers.ToPersianDate(presence.LastSeenAt),
                UpdatedAt = DateHelpers.ToPersianDate(presence.UpdatedAt)
            };

            return ServiceResult<object>.Ok(
                response,
                _localizer["UserPresenceUpdatedSuccessfully"]);
        }
        catch (Exception ex)
        {
            SecurityHelpers.InsertToLog(
                _logOpt.InsertLog,
                "UpdateUserPresenceAsync - " + ex);

            return ServiceResult<object>.Fail(
                _localizer["InternalServerError"], 500, ex.ToString());
        }
    }
}