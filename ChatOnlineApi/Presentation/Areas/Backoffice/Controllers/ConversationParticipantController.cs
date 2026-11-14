
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Extensions;
using ChatOnlineApi.Application.Contracts;
using static ChatOnlineApi.Application.DTOs.ConversationParticipantDtos;

namespace ChatOnlineApi.Presentation.Areas.Backoffice.Controllers;

[ApiController]
[Route("backoffice/[controller]")]
[ApiExplorerSettings(GroupName = "backoffice")]
public class ConversationParticipantController(
    IConversationParticipantService _participant) : ControllerBase
{
    /// <summary>
    /// اضافه کردن عضو به گفتگو
    /// </summary>
    [HttpPost("Add")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> Add(
        [FromBody] AddConversationParticipantRequest participant)
        => this.ToActionResult(await _participant.AddAsync(participant));

    /// <summary>
    /// ویرایش اطلاعات عضو گفتگو
    /// </summary>
    [HttpPost("Edit")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> Edit(
        [FromBody] EditConversationParticipantRequest participant)
        => this.ToActionResult(await _participant.EditAsync(participant));

    /// <summary>
    /// خارج کردن عضو از گفتگو
    /// </summary>
    [HttpGet("Delete")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> Delete([FromHeader] int participantId)
        => this.ToActionResult(await _participant.DeleteAsync(participantId));

    /// <summary>
    /// دریافت اطلاعات یک عضو گفتگو
    /// </summary>
    [HttpGet("Get")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> Get([FromHeader] int participantId)
        => this.ToActionResult(await _participant.GetAsync(participantId));

    /// <summary>
    /// دریافت لیست اعضای گفتگو
    /// </summary>
    [HttpPost("GetAll")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> GetAll([FromBody] ShowConversationParticipantRequest participantRequest,[FromHeader] int pageSize,[FromHeader] int pageNumber)
        => this.ToActionResult(
            await _participant.GetAllAsync(participantRequest, pageSize, pageNumber));
}
