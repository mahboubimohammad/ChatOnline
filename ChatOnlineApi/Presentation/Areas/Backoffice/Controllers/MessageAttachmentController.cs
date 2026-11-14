
using ChatOnlineApi.Application.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Extensions;
using static ChatOnlineApi.Application.DTOs.MessageAttachmentDtos;

namespace ChatOnlineApi.Presentation.Areas.Backoffice.Controllers;

[ApiController]
[Route("backoffice/[controller]")]
[ApiExplorerSettings(GroupName = "backoffice")]
public class MessageAttachmentController(
    IMessageAttachmentService _messageAttachment) : ControllerBase
{
    /// <summary>
    /// افزودن فایل پیوست به پیام
    /// </summary>
    [HttpPost("Add")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> Add(
        [FromBody] AddMessageAttachmentRequest request)
        => this.ToActionResult(await _messageAttachment.AddAsync(request));

    /// <summary>
    /// ویرایش نام فایل پیوست
    /// </summary>
    [HttpPost("Edit")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> Edit(
        [FromBody] EditMessageAttachmentRequest request)
        => this.ToActionResult(await _messageAttachment.EditAsync(request));

    /// <summary>
    /// حذف فایل پیوست
    /// </summary>
    [HttpGet("Delete")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> Delete(
        [FromHeader] int attachmentId)
        => this.ToActionResult(await _messageAttachment.DeleteAsync(attachmentId));

    /// <summary>
    /// دریافت لیست فایل‌های پیوست
    /// </summary>
    [HttpPost("GetAll")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> GetAll([FromBody] ShowMessageAttachmentRequest filter,[FromHeader] int pageSize ,[FromHeader] int pageNumber )
        => this.ToActionResult(await _messageAttachment.GetAllAsync(filter, pageSize, pageNumber));

    /// <summary>
    /// دریافت فایل پیوست بر اساس شناسه
    /// </summary>
    [HttpGet("Get")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> Get([FromHeader] int attachmentId)
        => this.ToActionResult(await _messageAttachment.GetAsync(attachmentId));
}
