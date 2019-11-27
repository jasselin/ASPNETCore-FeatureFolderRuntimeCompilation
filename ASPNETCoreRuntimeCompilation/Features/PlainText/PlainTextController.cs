using Microsoft.AspNetCore.Mvc;

namespace ASPNETCoreRuntimeCompilation.Features.PlainText
{
    public class PlainTextController : Controller
    {
        public IActionResult Index()
        {
            return Content("Text content!");
        }
    }
}
