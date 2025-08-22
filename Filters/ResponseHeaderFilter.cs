using Microsoft.AspNetCore.Mvc.Filters;

namespace ControllerExample.Filters;

public class ResponseHeaderFilter : IActionFilter
{
    private readonly ILogger<ResponseHeaderFilter> _logger;
    private readonly string _key;
    private readonly string _value;

    public ResponseHeaderFilter(ILogger<ResponseHeaderFilter> logger, string key, string value)
    {
        _logger = logger;
        _key = key;
        _value = value;
    }
    public void OnActionExecuted(ActionExecutedContext context)
    {
        _logger.LogInformation("{Filter}.{Method}", nameof(ResponseHeaderFilter), nameof(OnActionExecuted));
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        _logger.LogInformation("{Filter}.{Method}", nameof(ResponseHeaderFilter), nameof(OnActionExecuted));

        context.HttpContext.Response.Headers[_key] = _value;
    }
}
