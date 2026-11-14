
using ChatOnlineApi.Application.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Extensions;
using static ChatOnlineApi.Application.DTOs.MessageMentionDtos;

namespace ChatOnlineApi.Presentation.Areas.Backoffice.Controllers;

[ApiController]
[Route("backoffice/[controller]")]
[ApiExplorerSettings(GroupName = "backoffice")]
public class MessageMentionController(
    IMessageMentionService _messageMention) : ControllerBase
{
    /// <summary>
    /// افزودن اشاره به کاربر در پیام
    /// </summary>
    [HttpPost("Add")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> Add([FromBody] AddMessageMentionRequest request)
        => this.ToActionResult(await _messageMention.AddAsync(request));

    /// <summary>
    /// حذف اشاره به کاربر
    /// </summary>
    [HttpGet("Delete")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> Delete([FromHeader] int mentionId)
        => this.ToActionResult(await _messageMention.DeleteAsync(mentionId));

    /// <summary>
    /// دریافت لیست اشاره‌های پیام
    /// </summary>
    [HttpPost("GetAll")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> GetAll(
        [FromBody] ShowMessageMentionRequest filter,
        [FromHeader] int pageSize ,
        [FromHeader] int pageNumber )
        => this.ToActionResult(
            await _messageMention.GetAllAsync(filter, pageSize, pageNumber));

    /// <summary>
    /// دریافت اشاره به کاربر بر اساس شناسه
    /// </summary>
    [HttpGet("Get")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> Get([FromHeader] int mentionId)
        => this.ToActionResult(await _messageMention.GetAsync(mentionId));
}
