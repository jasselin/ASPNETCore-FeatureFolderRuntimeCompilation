using Microsoft.AspNetCore.Mvc;

namespace ASPNETCoreRuntimeCompilation.Features.Nested.FeatureC
{
    public class FeatureCController : Controller
    {
        public IActionResult Index()
        {
            var viewModel = new FeatureCViewModel
            {
                Message = "Feature C (Nested) message"
            };

            return View("FeatureC", viewModel);
        }
    }
}
