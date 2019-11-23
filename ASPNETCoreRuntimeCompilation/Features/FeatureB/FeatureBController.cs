using Microsoft.AspNetCore.Mvc;

namespace ASPNETCoreRuntimeCompilation.Features.FeatureB
{
    public class FeatureBController : Controller
    {
        public IActionResult Index()
        {
            var viewModel = new FeatureBViewModel
            {
                Message = "Feature B message"
            };

            return View("FeatureB", viewModel);
        }
    }
}
