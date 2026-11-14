using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Common;

namespace SharedLibrary.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToActionResult<T>(this ControllerBase c, ServiceResult<T> r)
    {
        if (r.Data != null)
        {
            return c.StatusCode(r.StatusCode, new
            {
                message = r.Message,
                value = r.Data,
                error = r.Error
            });
        }

        return c.StatusCode(r.StatusCode, new
        {
            message = r.Message,
            error = r.Error
        });
    }
}
