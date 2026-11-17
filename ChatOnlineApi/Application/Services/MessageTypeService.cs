
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using SharedLibrary.Common;
using SharedLibrary.Helpers;
using SharedLibrary.Models.DTOs;
using ChatOnlineApi.Application.Contracts;
using ChatOnlineApi.Domain.Base;
using ChatOnlineApi.Domain.Interfaces;
using ChatOnlineApi.Infrastructure.Configuration;
using static ChatOnlineApi.Application.DTOs.MessageTypeDtos;

namespace ChatOnlineApi.Application.Services;

public class MessageTypeService(
    IMessageTypeRepository _types,
    IMessageRepository _messages,
    IUnitOfWork _unitOfWork,
    IStringLocalizer<MessageTypeService> _localizer,
    IOptions<LogOptions> logOpt) : IMessageTypeService
{
    private readonly LogOptions _logOpt = logOpt.Value;

    public async Task<ServiceResult<object>> GetAllAsync()
    {
        try
        {
            var items = await _types.Get()
                .OrderBy(x => x.Id)
                .Select(x => new ShowMessageType
                {
                    Id = x.Id,
                    Title = x.Title
                })
                .ToListAsync();

            return ServiceResult<object>.Ok(items);
        }
        catch (Exception ex)
        {
            SecurityHelpers.InsertToLog(
                _logOpt.InsertLog,
                "GetAllMessageTypesAsync - " + ex);

            return ServiceResult<object>.Fail(
                _localizer["InternalServerError"], 500, ex.ToString());
        }
    }
    public async Task<ServiceResult<List<DropDownDTO>>> DropDownAsync()
    {
        try
        {
            var items = await _types.Get()
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
                "DropDownMessageTypesAsync - " + ex);

            return ServiceResult<List<DropDownDTO>>.Fail(
                _localizer["InternalServerError"], 500, ex.ToString());
        }
    }
}