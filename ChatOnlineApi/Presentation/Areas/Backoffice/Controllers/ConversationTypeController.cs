
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Extensions;
using ChatOnlineApi.Application.Contracts;
using static ChatOnlineApi.Application.DTOs.ConversationTypeDtos;

namespace ChatOnlineApi.Presentation.Areas.Backoffice.Controllers;

[ApiController]
[Route("backoffice/[controller]")]
[ApiExplorerSettings(GroupName = "backoffice")]
public class ConversationTypeController(
    IConversationTypeService _conversationType) : ControllerBase
{

    /// <summary>
    /// دریافت اطلاعات یک نوع گفتگو
    /// </summary>
    [HttpGet("Get")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> Get(
        [FromHeader] int conversationTypeId)
        => this.ToActionResult(await _conversationType.GetAsync(conversationTypeId));

}

