using Microsoft.AspNetCore.Mvc;

namespace ControllerExample.ViewComponents
{
    public class GridViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(string title)
        {
            return View("Default", title);
        }
    }
}