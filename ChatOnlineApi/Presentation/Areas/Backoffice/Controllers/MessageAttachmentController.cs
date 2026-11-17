
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Extensions;
using ChatOnlineApi.Application.Contracts;
using static ChatOnlineApi.Application.DTOs.MessageAttachmentDtos;

namespace ChatOnlineApi.Presentation.Areas.Backoffice.Controllers;

[ApiController]
[Route("backoffice/[controller]")]
[ApiExplorerSettings(GroupName = "backoffice")]
public class MessageAttachmentController(
    IMessageAttachmentService _messageAttachment) : ControllerBase
{
    /// <summary>
    /// افزودن پیوست به پیام
    /// </summary>
    [HttpPost("Add")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> Add(
        [FromBody] AddMessageAttachmentRequest request)
        => this.ToActionResult(await _messageAttachment.AddAsync(request));

    /// <summary>
    /// ویرایش اطلاعات پیوست
    /// </summary>
    [HttpPost("Edit")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> Edit(
        [FromBody] EditMessageAttachmentRequest request)
        => this.ToActionResult(await _messageAttachment.EditAsync(request));

    /// <summary>
    /// حذف پیوست
    /// </summary>
    [HttpGet("Delete")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> Delete(
        [FromHeader] int attachmentId)
        => this.ToActionResult(await _messageAttachment.DeleteAsync(attachmentId));

    /// <summary>
    /// دریافت اطلاعات یک پیوست
    /// </summary>
    [HttpGet("Get")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> Get(
        [FromHeader] int attachmentId)
        => this.ToActionResult(await _messageAttachment.GetAsync(attachmentId));

    /// <summary>
    /// دریافت لیست پیوست‌ها
    /// </summary>
    [HttpPost("GetAll")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> GetAll([FromBody] ShowMessageAttachmentRequest request,[FromHeader] int pageSize,[FromHeader] int pageNumber)
        => this.ToActionResult(await _messageAttachment.GetAllAsync(request, pageSize, pageNumber));
}
