using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Extensions;
using ChatOnlineApi.Application.Contracts;
using static ChatOnlineApi.Application.DTOs.ConversationDtos;

namespace ChatOnlineApi.Presentation.Areas.Backoffice.Controllers;

[ApiController]
[Route("backoffice/[controller]")]
[ApiExplorerSettings(GroupName = "backoffice")]
public class ConversationController(IConversationService _conversation) : ControllerBase
{
    /// <summary>
    /// ایجاد گفتگوی جدید
    /// </summary>
    [HttpPost("Add")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> Add([FromBody] AddConversationRequest conversation)
        => this.ToActionResult(await _conversation.AddAsync(conversation));

    /// <summary>
    /// ویرایش اطلاعات گفتگو
    /// </summary>
    [HttpPost("Edit")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> Edit([FromBody] EditConversationRequest conversation)
        => this.ToActionResult(await _conversation.EditAsync(conversation));

    /// <summary>
    /// حذف گفتگو
    /// </summary>
    [HttpGet("Delete")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> Delete([FromHeader] int conversationId)
        => this.ToActionResult(await _conversation.DeleteAsync(conversationId));

    /// <summary>
    /// دریافت اطلاعات یک گفتگو
    /// </summary>
    [HttpGet("Get")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> Get([FromHeader] int conversationId)
        => this.ToActionResult(await _conversation.GetAsync(conversationId));
    /// <summary>
    /// دریافت لیست گفتگوها
    /// </summary>
    [HttpPost("GetAll")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> GetAll([FromBody] ShowConversationRequest conversationRequest,[FromHeader] int pageSize,[FromHeader] int pageNumber)
        => this.ToActionResult(await _conversation.GetAllAsync(conversationRequest, pageSize, pageNumber));

}