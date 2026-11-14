
using ChatOnlineApi.Application.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Extensions;
using static ChatOnlineApi.Application.DTOs.MessageReactionDtos;

namespace ChatOnlineApi.Presentation.Areas.Backoffice.Controllers;

[ApiController]
[Route("backoffice/[controller]")]
[ApiExplorerSettings(GroupName = "backoffice")]
public class MessageReactionController(
    IMessageReactionService _messageReaction) : ControllerBase
{
    /// <summary>
    /// افزودن یا تغییر واکنش به پیام
    /// </summary>
    [HttpPost("Add")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> Add([FromBody] AddMessageReactionRequest request)
        => this.ToActionResult(await _messageReaction.AddAsync(request));

    /// <summary>
    /// حذف واکنش
    /// </summary>
    [HttpGet("Delete")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> Delete([FromHeader] int reactionId)
        => this.ToActionResult(await _messageReaction.DeleteAsync(reactionId));

    /// <summary>
    /// دریافت واکنش بر اساس شناسه
    /// </summary>
    [HttpGet("Get")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> Get([FromHeader] int reactionId)
        => this.ToActionResult(await _messageReaction.GetAsync(reactionId));

    /// <summary>
    /// دریافت لیست واکنش‌ها
    /// </summary>
    [HttpPost("GetAll")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> GetAll([FromBody] ShowMessageReactionRequest filter,[FromHeader] int pageSize ,[FromHeader] int pageNumber )
        => this.ToActionResult(
            await _messageReaction.GetAllAsync(filter, pageSize, pageNumber));
}
