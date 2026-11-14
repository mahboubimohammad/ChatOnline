
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
    /// اضافه کردن نوع گفتگو
    /// </summary>
    [HttpPost("Add")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> Add(
        [FromBody] AddConversationTypeRequest request)
        => this.ToActionResult(await _conversationType.AddAsync(request));

    /// <summary>
    /// ویرایش نوع گفتگو
    /// </summary>
    [HttpPost("Edit")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> Edit(
        [FromBody] EditConversationTypeRequest request)
        => this.ToActionResult(await _conversationType.EditAsync(request));

    /// <summary>
    /// حذف نوع گفتگو
    /// </summary>
    [HttpGet("Delete")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> Delete(
        [FromHeader] int conversationTypeId)
        => this.ToActionResult(await _conversationType.DeleteAsync(conversationTypeId));

    /// <summary>
    /// دریافت اطلاعات یک نوع گفتگو
    /// </summary>
    [HttpGet("Get")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> Get(
        [FromHeader] int conversationTypeId)
        => this.ToActionResult(await _conversationType.GetAsync(conversationTypeId));

    /// <summary>
    /// دریافت لیست انواع گفتگو
    /// </summary>
    [HttpPost("GetAll")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> GetAll([FromBody] ShowConversationTypeRequest request,[FromHeader] int pageSize,[FromHeader] int pageNumber)
        => this.ToActionResult(
            await _conversationType.GetAllAsync(request, pageSize, pageNumber));
}

