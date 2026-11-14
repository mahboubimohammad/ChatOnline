
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Extensions;
using ChatOnlineApi.Application.Contracts;
using static ChatOnlineApi.Application.DTOs.ConversationPinDtos;

namespace ChatOnlineApi.Presentation.Areas.Backoffice.Controllers;

[ApiController]
[Route("backoffice/[controller]")]
[ApiExplorerSettings(GroupName = "backoffice")]
public class ConversationPinController(
    IConversationPinService _conversationPin) : ControllerBase
{
    /// <summary>
    /// سنجاق کردن پیام در گفتگو
    /// </summary>
    [HttpPost("Add")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> Add(
        [FromBody] AddConversationPinRequest request)
        => this.ToActionResult(await _conversationPin.AddAsync(request));

    /// <summary>
    /// حذف پیام از حالت سنجاق‌شده
    /// </summary>
    [HttpGet("Delete")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> Delete([FromHeader] int pinId)
        => this.ToActionResult(await _conversationPin.DeleteAsync(pinId));

    /// <summary>
    /// دریافت اطلاعات یک پیام سنجاق‌شده
    /// </summary>
    [HttpGet("Get")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> Get([FromHeader] int pinId)
        => this.ToActionResult(await _conversationPin.GetAsync(pinId));

    /// <summary>
    /// دریافت لیست پیام‌های سنجاق‌شده
    /// </summary>
    [HttpPost("GetAll")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> GetAll([FromBody] ShowConversationPinRequest pinRequest,[FromHeader] int pageSize,[FromHeader] int pageNumber)
        => this.ToActionResult(await _conversationPin.GetAllAsync(pinRequest, pageSize, pageNumber));
}
