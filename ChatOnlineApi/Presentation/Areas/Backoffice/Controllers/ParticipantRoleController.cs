using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Extensions;
using ChatOnlineApi.Application.Contracts;
using static ChatOnlineApi.Application.DTOs.ParticipantRoleDtos;

namespace ChatOnlineApi.Presentation.Areas.Backoffice.Controllers;

[ApiController]
[Route("backoffice/[controller]")]
[ApiExplorerSettings(GroupName = "backoffice")]
public class ParticipantRoleController(
    IParticipantRoleService _participantRoleService) : ControllerBase
{

    /// <summary>
    /// دریافت نقش‌ها برای دراپ‌داون
    /// </summary>
    [HttpGet("GetDropDown")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> GetDropDown()
        => this.ToActionResult(
            await _participantRoleService.GetDropDownAsync());
    /// <summary>
    /// دریافت لیست نقش‌های شرکت‌کنندگان
    /// </summary>
    [HttpPost("GetAll")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> GetAll([FromBody] ShowParticipantRoleRequest request,[FromHeader] int pageSize ,[FromHeader] int pageNumber )
        => this.ToActionResult(
            await _participantRoleService.GetAllAsync(request, pageSize, pageNumber));
}