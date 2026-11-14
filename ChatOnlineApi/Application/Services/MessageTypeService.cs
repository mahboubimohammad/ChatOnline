using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using SharedLibrary.Common;
using SharedLibrary.Helpers;
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

    public async Task<ServiceResult<object>> AddAsync(AddMessageTypeRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Title))
                return ServiceResult<object>.Fail(_localizer["TitleRequired"], 400);

            var title = request.Title.Trim();

            var exists = await _types
                .Get(x => x.Title.ToLower() == title.ToLower())
                .AnyAsync();

            if (exists)
                return ServiceResult<object>.Fail(_localizer["MessageTypeAlreadyExists"], 409);

            var messageType = new Domain.Entities.MessageType
            {
                Title = title
            };

            await _types.CreateAsync(messageType);
            await _unitOfWork.SaveChangesAsync();

            var response = new ShowMessageTypeResponse
            {
                Id = messageType.Id,
                Title = messageType.Title
            };

            return ServiceResult<object>.Ok(
                response,
                _localizer["MessageTypeAddedSuccessfully"]);
        }
        catch (Exception ex)
        {
            SecurityHelpers.InsertToLog(
                _logOpt.InsertLog,
                "AddMessageTypeAsync - " + ex);

            return ServiceResult<object>.Fail(
                _localizer["InternalServerError"], 500, ex.ToString());
        }
    }

    public async Task<ServiceResult<object>> EditAsync(EditMessageTypeRequest request)
    {
        try
        {
            if (request.Id <= 0)
                return ServiceResult<object>.Fail(_localizer["InvalidId"], 400);

            if (string.IsNullOrWhiteSpace(request.Title))
                return ServiceResult<object>.Fail(_localizer["TitleRequired"], 400);

            var messageType = await _types.GetByIdAsync(request.Id);

            if (messageType == null)
                return ServiceResult<object>.Fail(_localizer["MessageTypeNotFound"], 404);

            var title = request.Title.Trim();

            var exists = await _types
                .Get(x => x.Id != request.Id &&
                          x.Title.ToLower() == title.ToLower())
                .AnyAsync();

            if (exists)
                return ServiceResult<object>.Fail(_localizer["MessageTypeAlreadyExists"], 409);

            messageType.Title = title;

            await _types.UpdateAsync(messageType);
            await _unitOfWork.SaveChangesAsync();

            var response = new ShowMessageTypeResponse
            {
                Id = messageType.Id,
                Title = messageType.Title
            };

            return ServiceResult<object>.Ok(
                response,
                _localizer["MessageTypeUpdatedSuccessfully"]);
        }
        catch (Exception ex)
        {
            SecurityHelpers.InsertToLog(
                _logOpt.InsertLog,
                "EditMessageTypeAsync - " + ex);

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

            var messageType = await _types.GetByIdAsync(id);

            if (messageType == null)
                return ServiceResult<object>.Fail(_localizer["MessageTypeNotFound"], 404);

            var isUsed = await _messages
                .Get(x => x.MessageTypeId == id)
                .AnyAsync();

            if (isUsed)
                return ServiceResult<object>.Fail(
                    _localizer["MessageTypeIsInUse"], 409);

            await _types.RemoveAsync(messageType);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<object>.Ok(
                _localizer["MessageTypeDeletedSuccessfully"]);
        }
        catch (Exception ex)
        {
            SecurityHelpers.InsertToLog(
                _logOpt.InsertLog,
                "DeleteMessageTypeAsync - " + ex);

            return ServiceResult<object>.Fail(
                _localizer["InternalServerError"], 500, ex.ToString());
        }
    }

    public async Task<ServiceResult<object>> GetAllAsync(ShowMessageTypeRequest request,int pageSize,int pageNumber)
    {
        try
        {
            var query = _types.Get();

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
                .Select(x => new ShowMessageTypeResponse
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
                "GetAllMessageTypesAsync - " + ex);

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

            var messageType = await _types.GetByIdAsync(id);

            if (messageType == null)
                return ServiceResult<object>.Fail(_localizer["MessageTypeNotFound"], 404);

            var response = new ShowMessageTypeResponse
            {
                Id = messageType.Id,
                Title = messageType.Title
            };

            return ServiceResult<object>.Ok(response);
        }
        catch (Exception ex)
        {
            SecurityHelpers.InsertToLog(
                _logOpt.InsertLog,
                "GetMessageTypeAsync - " + ex);

            return ServiceResult<object>.Fail(
                _localizer["InternalServerError"], 500, ex.ToString());
        }
    }
}