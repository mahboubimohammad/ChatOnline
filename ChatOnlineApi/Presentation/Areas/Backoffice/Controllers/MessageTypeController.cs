using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Extensions;
using ChatOnlineApi.Application.Contracts;

namespace ChatOnlineApi.Presentation.Areas.Backoffice.Controllers;

[ApiController]
[Route("backoffice/[controller]")]
[ApiExplorerSettings(GroupName = "backoffice")]
public class MessageTypeController(IMessageTypeService _messageType) : ControllerBase
{
    /// <summary>
    /// دریافت لیست انواع پیام
    /// </summary>
    [HttpGet("GetAll")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> GetAll()
        => this.ToActionResult(await _messageType.GetAllAsync());

    /// <summary>
    /// دریافت لیست انواع پیام 
    /// </summary>
    [HttpGet("DropDown")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> DropDown()
        => this.ToActionResult(await _messageType.DropDownAsync());
}
