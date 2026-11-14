using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Extensions;
using ChatOnlineApi.Application.Contracts;
using static ChatOnlineApi.Application.DTOs.NotificationDtos;

namespace ChatOnlineApi.Presentation.Areas.Backoffice.Controllers;

[ApiController]
[Route("backoffice/[controller]")]
[ApiExplorerSettings(GroupName = "backoffice")]
public class NotificationController(
    INotificationService _notificationService) : ControllerBase
{
    /// <summary>
    /// ایجاد اعلان
    /// </summary>
    [HttpPost("Add")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> Add([FromBody] AddNotificationRequest request)
        => this.ToActionResult(await _notificationService.AddAsync(request));

    /// <summary>
    /// دریافت لیست اعلان‌ها
    /// </summary>
    [HttpPost("GetAll")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> GetAll([FromBody] ShowNotificationRequest request,[FromHeader] int pageSize,[FromHeader] int pageNumber )
        => this.ToActionResult(await _notificationService.GetAllAsync(request, pageSize, pageNumber));

    /// <summary>
    /// دریافت اعلان با شناسه
    /// </summary>
    [HttpGet("Get")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> Get([FromHeader] int id)
        => this.ToActionResult(await _notificationService.GetAsync(id));

    /// <summary>
    /// علامت‌گذاری اعلان به‌عنوان خوانده‌شده
    /// </summary>
    [HttpPost("MarkAsRead")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> MarkAsRead([FromHeader] int id)
        => this.ToActionResult(await _notificationService.MarkAsReadAsync(id));

    /// <summary>
    /// حذف اعلان
    /// </summary>
    [HttpGet("Delete")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> Delete([FromHeader] int id)
        => this.ToActionResult(await _notificationService.DeleteAsync(id));
}