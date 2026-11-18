
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
    public async Task<IActionResult> Add([FromBody] AddConversationRequest request)
        => this.ToActionResult(await _conversation.AddAsync(request));


    /// <summary>
    /// اختصاص گفتگو به پشتیبان
    /// </summary>
    [HttpPost("AssignSupportAgent")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> AssignSupportAgent([FromHeader] int conversationId,[FromHeader] int supportAgentId)
        => this.ToActionResult(await _conversation.AssignSupportAgentAsync(conversationId, supportAgentId));

    /// <summary>
    /// ویرایش اطلاعات گفتگو
    /// </summary>
    [HttpPost("Edit")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> Edit([FromBody] EditConversationRequest request)
        => this.ToActionResult(await _conversation.EditAsync(request));

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
    public async Task<IActionResult> GetAll([FromBody] ShowConversationRequest request,[FromHeader] int pageSize,[FromHeader] int pageNumber)
        => this.ToActionResult(await _conversation.GetAllAsync(request, pageSize, pageNumber));
}
