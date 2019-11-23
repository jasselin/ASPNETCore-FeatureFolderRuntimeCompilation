using Microsoft.AspNetCore.Mvc;

namespace ASPNETCoreRuntimeCompilation.Features.Nested.FeatureA
{
    public class FeatureAController : Controller
    {
        public IActionResult Index()
        {
            var viewModel = new FeatureAViewModel
            {
                Message = "Feature A (Nested) message"
            };

            return View("FeatureA", viewModel);
        }
    }
}
