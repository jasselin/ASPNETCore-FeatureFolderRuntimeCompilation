using Microsoft.AspNetCore.Mvc;

namespace ASPNETCoreRuntimeCompilation.Features.Nested.MoreNested.FeatureA
{
    public class FeatureAController : Controller
    {
        public IActionResult Index()
        {
            var viewModel = new FeatureAViewModel
            {
                Message = "Feature A (More nested) message"
            };

            return View("FeatureA", viewModel);
        }
    }
}
