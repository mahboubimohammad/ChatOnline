
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Extensions;
using ChatOnlineApi.Application.Contracts;
using static ChatOnlineApi.Application.DTOs.MessageDtos;

namespace ChatOnlineApi.Presentation.Areas.Backoffice.Controllers;

[ApiController]
[Route("backoffice/[controller]")]
[ApiExplorerSettings(GroupName = "backoffice")]
public class MessageController(IMessageService _message) : ControllerBase
{
    /// <summary>
    /// ارسال پیام جدید
    /// </summary>
    [HttpPost("Add")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> Add([FromBody] AddMessageRequest request)
        => this.ToActionResult(await _message.AddAsync(request));

    /// <summary>
    /// ویرایش پیام
    /// </summary>
    [HttpPost("Edit")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> Edit([FromBody] EditMessageRequest request)
        => this.ToActionResult(await _message.EditAsync(request));

    /// <summary>
    /// حذف پیام
    /// </summary>
    [HttpGet("Delete")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> Delete([FromHeader] int messageId)
        => this.ToActionResult(await _message.DeleteAsync(messageId));

    /// <summary>
    /// دریافت اطلاعات یک پیام
    /// </summary>
    [HttpGet("Get")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> Get([FromHeader] int messageId)
        => this.ToActionResult(await _message.GetAsync(messageId));

    /// <summary>
    /// دریافت لیست پیام‌ها
    /// </summary>
    [HttpPost("GetAll")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> GetAll([FromBody] ShowMessageRequest request,[FromHeader] int pageSize,[FromHeader] int pageNumber)
        => this.ToActionResult(await _message.GetAllAsync(request, pageSize, pageNumber));
}
