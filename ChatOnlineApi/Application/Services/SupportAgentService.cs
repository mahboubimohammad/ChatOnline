using ChatOnlineApi.Application.Contracts;
using ChatOnlineApi.Application.DTOs;
using ChatOnlineApi.Domain.Base;
using ChatOnlineApi.Domain.Entities;
using ChatOnlineApi.Domain.Interfaces;
using ChatOnlineApi.Infrastructure.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using SharedLibrary.Common;
using SharedLibrary.Helpers;

namespace ChatOnlineApi.Application.Services;

public class SupportAgentService(
    ISupportAgentRepository _supportAgents,
    IUserPresenceRepository _userPresences,
    IUnitOfWork _unitOfWork,
    IStringLocalizer<SupportAgentService> _localizer,
    IOptions<LogOptions> logOpt) : ISupportAgentService
{
    private readonly LogOptions _logOpt = logOpt.Value;

    public async Task<ServiceResult<object>> AddAsync(AddSupportAgentRequest request)
    {
        try
        {
            if (!request.UserId.HasValue || request.UserId <= 0)
                return ServiceResult<object>.Fail("شناسه کاربر معتبر نیست.", 400);

            if (string.IsNullOrWhiteSpace(request.FullName))
                return ServiceResult<object>.Fail("نام پشتیبان الزامی است.", 400);

            var exists = await _supportAgents.Get()
                .AnyAsync(x => x.UserId == request.UserId.Value);

            if (exists)
                return ServiceResult<object>.Fail("این کاربر قبلاً به‌عنوان پشتیبان ثبت شده است.", 400);

            var agent = new SupportAgent
            {
                UserId = request.UserId.Value,
                FullName = request.FullName.Trim(),
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            await _supportAgents.CreateAsync(agent);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<object>.Ok(new
            {
                agent.Id,
                agent.UserId,
                agent.FullName,
                agent.IsActive,
                CreatedAt = DateHelpers.ToPersianDate(agent.CreatedAt)
            });
        }
        catch (Exception ex)
        {
            SecurityHelpers.InsertToLog(
                _logOpt.InsertLog,
                "AddSupportAgentAsync - " + ex);

            return ServiceResult<object>.Fail(
                _localizer["InternalServerError"],
                500,
                ex.ToString());
        }
    }
  public async Task<ServiceResult<object>> GetAsync()
    {
        try
        {
            var agents = await _supportAgents.Get()
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .ToListAsync();

            var userIds = agents.Select(x => x.UserId).ToList();

            var presences = await _userPresences.Get()
                .AsNoTracking()
                .Where(x => userIds.Contains(x.UserId))
                .ToDictionaryAsync(x => x.UserId);

            var items = agents.Select(x =>
            {
                presences.TryGetValue(x.UserId, out var presence);

                return new ShowSupportAgentResponse
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    FullName = x.FullName,
                    IsActive = x.IsActive,
                    IsOnline = presence?.IsOnline ?? false,
                    LastSeenAt = DateHelpers.ToPersianDate(presence?.LastSeenAt),
                    CreatedAt = DateHelpers.ToPersianDate(x.CreatedAt)
                };
            }).ToList();

            return ServiceResult<object>.Ok(items);
        }
        catch (Exception ex)
        {
            SecurityHelpers.InsertToLog(
                _logOpt.InsertLog,
                "GetSupportAgentsAsync - " + ex);

            return ServiceResult<object>.Fail(
                _localizer["InternalServerError"],
                500,
                ex.ToString());
        }
    }
}