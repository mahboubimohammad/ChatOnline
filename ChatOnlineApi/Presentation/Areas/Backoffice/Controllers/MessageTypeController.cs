using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Extensions;
using ChatOnlineApi.Application.Contracts;
using ChatOnlineApi.Application.DTOs;
using static ChatOnlineApi.Application.DTOs.MessageTypeDtos;

namespace ChatOnlineApi.Presentation.Areas.Backoffice.Controllers;

[ApiController]
[Route("backoffice/[controller]")]
[ApiExplorerSettings(GroupName = "backoffice")]
public class MessageTypeController(IMessageTypeService _messageTypeService) : ControllerBase
{
    /// <summary>
    /// افزودن نوع پیام
    /// </summary>
    [HttpPost("Add")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> Add([FromBody] AddMessageTypeRequest request)
        => this.ToActionResult(await _messageTypeService.AddAsync(request));

    /// <summary>
    /// ویرایش نوع پیام
    /// </summary>
    [HttpPost("Edit")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> Edit([FromBody] EditMessageTypeRequest request)
        => this.ToActionResult(await _messageTypeService.EditAsync(request));

    /// <summary>
    /// حذف نوع پیام
    /// </summary>
    [HttpGet("Delete")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> Delete([FromHeader] int id)
        => this.ToActionResult(await _messageTypeService.DeleteAsync(id));

    /// <summary>
    /// دریافت نوع پیام با شناسه
    /// </summary>
    [HttpGet("Get")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> Get([FromHeader] int id)
        => this.ToActionResult(await _messageTypeService.GetAsync(id));
    /// <summary>
    /// دریافت لیست انواع پیام
    /// </summary>
    [HttpPost("GetAll")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> GetAll([FromBody] ShowMessageTypeRequest request,[FromHeader] int pageSize ,[FromHeader] int pageNumber )
        => this.ToActionResult(
            await _messageTypeService.GetAllAsync(request, pageSize, pageNumber));

}