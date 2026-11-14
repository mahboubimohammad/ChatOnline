using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using SharedLibrary.Common;
using SharedLibrary.Helpers;
using SharedLibrary.Models.DTOs;
using ChatOnlineApi.Application.Contracts;
using ChatOnlineApi.Domain.Interfaces;
using ChatOnlineApi.Infrastructure.Configuration;
using static ChatOnlineApi.Application.DTOs.ParticipantRoleDtos;

namespace ChatOnlineApi.Application.Services;

public class ParticipantRoleService(
    IParticipantRoleRepository _roles,
    IStringLocalizer<ParticipantRoleService> _localizer,
    IOptions<LogOptions> logOpt) : IParticipantRoleService
{
    private readonly LogOptions _logOpt = logOpt.Value;

    public async Task<ServiceResult<object>> GetAllAsync(
        ShowParticipantRoleRequest request,
        int pageSize,
        int pageNumber)
    {
        try
        {
            var query = _roles.Get();

            if (request.Ids != null && request.Ids.Any())
                query = query.Where(x => request.Ids.Contains(x.Id));

            if (!string.IsNullOrWhiteSpace(request.Title))
            {
                var title = request.Title.Trim();
                query = query.Where(x => x.Title.Contains(title));
            }

            var totalCount = await query.CountAsync();

            pageSize = pageSize <= 0 ? 10 : pageSize;
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;

            var items = await query
                .OrderBy(x => x.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new ShowParticipantRoleResponse
                {
                    Id = x.Id,
                    Title = x.Title
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
                "GetAllParticipantRolesAsync - " + ex);

            return ServiceResult<object>.Fail(
                _localizer["InternalServerError"], 500, ex.ToString());
        }
    }

    public async Task<ServiceResult<List<DropDownDTO>>> GetDropDownAsync()
    {
        try
        {
            var items = await _roles.Get()
                .OrderBy(x => x.Id)
                .Select(x => new DropDownDTO
                {
                    Value = x.Id,
                    Label = x.Title
                })
                .ToListAsync();

            return ServiceResult<List<DropDownDTO>>.Ok(items);
        }
        catch (Exception ex)
        {
            SecurityHelpers.InsertToLog(
                _logOpt.InsertLog,
                "GetDropDownParticipantRolesAsync - " + ex);

            return ServiceResult<List<DropDownDTO>>.Fail(
                _localizer["InternalServerError"], 500, ex.ToString());
        }
    }
}