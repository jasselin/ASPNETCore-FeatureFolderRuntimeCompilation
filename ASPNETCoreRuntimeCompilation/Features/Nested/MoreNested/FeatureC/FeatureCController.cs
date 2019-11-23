using Microsoft.AspNetCore.Mvc;

namespace ASPNETCoreRuntimeCompilation.Features.Nested.MoreNested.FeatureC
{
    public class FeatureCController : Controller
    {
        public IActionResult Index()
        {
            var viewModel = new FeatureCViewModel
            {
                Message = "Feature C (More nested) message"
            };

            return View("FeatureC", viewModel);
        }
    }
}
