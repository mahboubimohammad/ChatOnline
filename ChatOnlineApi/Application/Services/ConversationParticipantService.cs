using ChatOnlineApi.Application.Contracts;
using ChatOnlineApi.Domain.Base;
using ChatOnlineApi.Domain.Entities;
using ChatOnlineApi.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedLibrary.Common;
using SharedLibrary.Helpers;
using static ChatOnlineApi.Application.DTOs.ConversationParticipantDtos;

namespace ChatOnlineApi.Application.Services;

public class ConversationParticipantService(
    IUnitOfWork _unitOfWork,
    IConversationParticipantRepository _participants,
    ILogger<ConversationParticipantService> _logger) : IConversationParticipantService
{
    public async Task<ServiceResult<object>> AddAsync(AddConversationParticipantRequest request)
    {
        try
        {
            if (request == null ||
                request.ConversationId <= 0 ||
                request.UserId <= 0 ||
                request.RoleId <= 0)
                return ServiceResult<object>.Fail("اطلاعات ارسالی معتبر نیست", 400);

            var exists = await _participants
                .Get(x => x.ConversationId == request.ConversationId &&
                          x.UserId == request.UserId &&
                          x.IsActive)
                .AnyAsync();

            if (exists)
                return ServiceResult<object>.Fail("این کاربر قبلاً به گفتگو اضافه شده است", 400);

            var participant = new ConversationParticipant
            {
                ConversationId = request.ConversationId,
                UserId = request.UserId,
                RoleId = request.RoleId,
                JoinedAt = DateTime.Now,
                IsActive = true,
                IsMuted = false,
                IsPinned = false,
                IsArchived = false
            };

            await _participants.CreateAsync(participant);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<object>.Ok(new ShowConversationParticipantResponse
            {
                Id = participant.Id,
                ConversationId = participant.ConversationId,
                UserId = participant.UserId,
                RoleId = participant.RoleId,
                JoinedAt = DateHelpers.ToPersianDate(participant.JoinedAt),
                LeftAt = DateHelpers.ToPersianDate(participant.LeftAt),
                IsActive = participant.IsActive,
                IsMuted = participant.IsMuted,
                IsPinned = participant.IsPinned,
                IsArchived = participant.IsArchived
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in AddConversationParticipant");

            return ServiceResult<object>.Fail(
                "خطای داخلی سرور رخ داده است", 500, ex.ToString());
        }
    }

    public async Task<ServiceResult<object>> EditAsync(EditConversationParticipantRequest request)
    {
        try
        {
            if (request == null || request.Id <= 0 || request.RoleId <= 0)
                return ServiceResult<object>.Fail("اطلاعات ارسالی معتبر نیست", 400);

            var participant = await _participants
                .Get(x => x.Id == request.Id)
                .FirstOrDefaultAsync();

            if (participant == null)
                return ServiceResult<object>.Fail("عضو گفتگو پیدا نشد", 404);

            participant.RoleId = request.RoleId;
            participant.IsActive = request.IsActive;
            participant.IsMuted = request.IsMuted;
            participant.IsPinned = request.IsPinned;
            participant.IsArchived = request.IsArchived;

            if (!request.IsActive && participant.LeftAt == null)
                participant.LeftAt = DateTime.Now;

            if (request.IsActive)
                participant.LeftAt = null;

            await _participants.UpdateAsync(participant);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<object>.Ok(new ShowConversationParticipantResponse
            {
                Id = participant.Id,
                ConversationId = participant.ConversationId,
                UserId = participant.UserId,
                RoleId = participant.RoleId,
                JoinedAt = DateHelpers.ToPersianDate(participant.JoinedAt),
                LeftAt = DateHelpers.ToPersianDate(participant.LeftAt),
                IsActive = participant.IsActive,
                IsMuted = participant.IsMuted,
                IsPinned = participant.IsPinned,
                IsArchived = participant.IsArchived
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in EditConversationParticipant");

            return ServiceResult<object>.Fail(
                "خطای داخلی سرور رخ داده است", 500, ex.ToString());
        }
    }

    public async Task<ServiceResult<object>> DeleteAsync(int participantId)
    {
        try
        {
            if (participantId <= 0)
                return ServiceResult<object>.Fail("شناسه معتبر نیست", 400);

            var participant = await _participants
                .Get(x => x.Id == participantId)
                .FirstOrDefaultAsync();

            if (participant == null)
                return ServiceResult<object>.Fail("عضو گفتگو پیدا نشد", 404);

            participant.IsActive = false;
            participant.LeftAt = DateTime.Now;

            await _participants.UpdateAsync(participant);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<object>.Ok("عضو با موفقیت از گفتگو خارج شد");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in DeleteConversationParticipant");

            return ServiceResult<object>.Fail(
                "خطای داخلی سرور رخ داده است", 500, ex.ToString());
        }
    }

    public async Task<ServiceResult<object>> GetAllAsync(ShowConversationParticipantRequest filter,int pageSize,int pageNumber)
    {
        try
        {
            if (pageSize <= 0 || pageNumber <= 0)
                return ServiceResult<object>.Fail("اطلاعات صفحه‌بندی معتبر نیست", 400);

            var query = _participants.Get();

            if (filter != null)
            {
                if (filter.ConversationId.HasValue)
                    query = query.Where(x =>
                        x.ConversationId == filter.ConversationId.Value);

                if (filter.UserId.HasValue)
                    query = query.Where(x =>
                        x.UserId == filter.UserId.Value);

                if (filter.RoleId.HasValue)
                    query = query.Where(x =>
                        x.RoleId == filter.RoleId.Value);

                if (filter.IsActive.HasValue)
                    query = query.Where(x =>
                        x.IsActive == filter.IsActive.Value);
            }

            var totalCount = await query.CountAsync();

            var participants = await query
                .OrderByDescending(x => x.JoinedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var response = participants.Select(x =>
                new ShowConversationParticipantResponse
                {
                    Id = x.Id,
                    ConversationId = x.ConversationId,
                    UserId = x.UserId,
                    RoleId = x.RoleId,
                    JoinedAt = DateHelpers.ToPersianDate(x.JoinedAt),
                    LeftAt = DateHelpers.ToPersianDate(x.LeftAt),
                    IsActive = x.IsActive,
                    IsMuted = x.IsMuted,
                    IsPinned = x.IsPinned,
                    IsArchived = x.IsArchived
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
            _logger.LogError(ex, "Error in GetAllConversationParticipants");

            return ServiceResult<object>.Fail(
                "خطای داخلی سرور رخ داده است", 500, ex.ToString());
        }
    }

    public async Task<ServiceResult<object>> GetAsync(int participantId)
    {
        try
        {
            if (participantId <= 0)
                return ServiceResult<object>.Fail("شناسه معتبر نیست", 400);

            var participant = await _participants
                .Get(x => x.Id == participantId)
                .FirstOrDefaultAsync();

            if (participant == null)
                return ServiceResult<object>.Fail("عضو گفتگو پیدا نشد", 404);

            var response = new ShowConversationParticipantResponse
            {
                Id = participant.Id,
                ConversationId = participant.ConversationId,
                UserId = participant.UserId,
                RoleId = participant.RoleId,
                JoinedAt = DateHelpers.ToPersianDate(participant.JoinedAt),
                LeftAt = DateHelpers.ToPersianDate(participant.LeftAt),
                IsActive = participant.IsActive,
                IsMuted = participant.IsMuted,
                IsPinned = participant.IsPinned,
                IsArchived = participant.IsArchived
            };

            return ServiceResult<object>.Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetConversationParticipant");

            return ServiceResult<object>.Fail(
                "خطای داخلی سرور رخ داده است", 500, ex.ToString());
        }
    }
}
