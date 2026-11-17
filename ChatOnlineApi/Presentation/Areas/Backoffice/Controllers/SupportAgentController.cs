
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Extensions;
using ChatOnlineApi.Application.Contracts;
using ChatOnlineApi.Application.DTOs;

namespace ChatOnlineApi.Presentation.Areas.Backoffice.Controllers;

[ApiController]
[Route("backoffice/[controller]")]
[ApiExplorerSettings(GroupName = "backoffice")]
public class SupportAgentController(ISupportAgentService _supportAgent) : ControllerBase
{
    /// <summary>
    /// افزودن  پشتیبانی
    /// </summary>
    [HttpPost("Add")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> Add(
        [FromBody] AddSupportAgentRequest request)
        => this.ToActionResult(await _supportAgent.AddAsync(request));

    /// <summary>
    /// دریافت اطلاعات  پشتیبانی
    /// </summary>
    [HttpGet("Get")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> Get()
        => this.ToActionResult(await _supportAgent.GetAsync());
}
