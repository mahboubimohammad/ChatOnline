using ChatOnlineApi.Application.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Extensions;
using static ChatOnlineApi.Application.DTOs.MessageReadReceiptDtos;

namespace ChatOnlineApi.Presentation.Areas.Backoffice.Controllers;

[ApiController]
[Route("backoffice/[controller]")]
[ApiExplorerSettings(GroupName = "backoffice")]
public class MessageReadReceiptController(
    IMessageReadReceiptService _receipt) : ControllerBase
{
    /// <summary>
    /// ثبت رسید تحویل پیام
    /// </summary>
    [HttpPost("Add")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> Add([FromBody] AddMessageReadReceiptRequest request)
        => this.ToActionResult(await _receipt.AddAsync(request));

    /// <summary>
    /// ثبت وضعیت خوانده شدن پیام
    /// </summary>
    [HttpPost("Edit")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> Edit([FromBody] EditMessageReadReceiptRequest request)
        => this.ToActionResult(await _receipt.EditAsync(request));

    /// <summary>
    /// حذف رسید پیام
    /// </summary>
    [HttpGet("Delete")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> Delete([FromHeader] int receiptId)
        => this.ToActionResult(await _receipt.DeleteAsync(receiptId));

    /// <summary>
    /// دریافت رسید پیام بر اساس شناسه
    /// </summary>
    [HttpGet("Get")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> Get([FromHeader] int receiptId)
        => this.ToActionResult(await _receipt.GetAsync(receiptId));
    /// <summary>
    /// دریافت لیست رسیدهای پیام
    /// </summary>
    [HttpPost("GetAll")]
    [Authorize(Policy = "BackofficeBearerPolicy")]
    public async Task<IActionResult> GetAll([FromBody] ShowMessageReadReceiptRequest filter,[FromHeader] int pageSize ,[FromHeader] int pageNumber )
        => this.ToActionResult(await _receipt.GetAllAsync(filter, pageSize, pageNumber));

}
