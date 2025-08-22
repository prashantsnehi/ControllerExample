using Microsoft.AspNetCore.Mvc.Filters;

namespace ControllerExample.Filters;

public class PersonListActionFilter : IActionFilter
{
    private readonly ILogger<PersonListActionFilter> _logger;
    public PersonListActionFilter(ILogger<PersonListActionFilter> logger)
    {
        _logger = logger;
    }
    public void OnActionExecuted(ActionExecutedContext context)
    {
        _logger.LogInformation("Action executed at {Time}", DateTime.UtcNow);
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        _logger.LogInformation("Action executing at {Time}", DateTime.UtcNow);
    }
}
