
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Extensions;
using ChatOnlineApi.Application.Contracts;
using static ChatOnlineApi.Application.DTOs.ReactionTypeDtos;

namespace ChatOnlineApi.Presentation.Areas.Backoffice.Controllers;

[ApiController]
[Route("backoffice/[controller]")]
[ApiExplorerSettings(GroupName = "backoffice")]
public class ReactionTypeController(IReactionTypeService _reactionType) : ControllerBase
{
    /// <summary>
    /// دریافت لیست انواع ری‌اکشن
    /// </summary>
    [HttpPost("GetAll")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> GetAll([FromBody] ShowReactionTypeRequest request,[FromHeader] int pageSize,[FromHeader] int pageNumber)
        => this.ToActionResult(await _reactionType.GetAllAsync(request, pageSize, pageNumber));

    /// <summary>
    /// دریافت لیست انواع ری‌اکشن 
    /// </summary>
    [HttpGet("DropDown")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> DropDown()
        => this.ToActionResult(await _reactionType.DropDownAsync());
}
