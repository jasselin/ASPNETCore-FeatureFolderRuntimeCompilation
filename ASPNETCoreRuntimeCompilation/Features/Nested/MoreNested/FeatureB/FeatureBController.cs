using Microsoft.AspNetCore.Mvc;

namespace ASPNETCoreRuntimeCompilation.Features.Nested.MoreNested.FeatureB
{
    public class FeatureBController : Controller
    {
        public IActionResult Index()
        {
            //var viewModel = new FeatureBViewModel
            //{
            //    Message = "Feature B message"
            //};

            //return View("FeatureB", viewModel);
            return Content(this.GetType().Assembly.Location);
        }
    }
}
