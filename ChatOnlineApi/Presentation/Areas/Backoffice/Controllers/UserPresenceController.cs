
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Extensions;
using ChatOnlineApi.Application.Contracts;
using static ChatOnlineApi.Application.DTOs.UserPresenceDtos;

namespace ChatOnlineApi.Presentation.Areas.Backoffice.Controllers;

[ApiController]
[Route("backoffice/[controller]")]
[ApiExplorerSettings(GroupName = "backoffice")]
public class UserPresenceController(IUserPresenceService _userPresence) : ControllerBase
{
    /// <summary>
    /// دریافت وضعیت حضور کاربران
    /// </summary>
    [HttpPost("GetAll")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> GetAll([FromBody] ShowUserPresenceRequest request,[FromHeader] int pageSize,[FromHeader] int pageNumber)
        => this.ToActionResult(await _userPresence.GetAllAsync(request, pageSize, pageNumber));

    /// <summary>
    /// دریافت وضعیت حضور یک کاربر
    /// </summary>
    [HttpGet("Get")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> Get([FromHeader] int userId)
        => this.ToActionResult(await _userPresence.GetAsync(userId));

    /// <summary>
    /// به‌روزرسانی وضعیت حضور کاربر
    /// </summary>
    [HttpPost("UpdatePresence")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> UpdatePresence(
        [FromBody] UpdateUserPresenceRequest request)
        => this.ToActionResult(await _userPresence.UpdatePresenceAsync(request));
}