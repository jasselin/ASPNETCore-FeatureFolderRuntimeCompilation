using Microsoft.AspNetCore.Mvc;

namespace ASPNETCoreRuntimeCompilation.Features.FeatureA
{
    public class FeatureAController : Controller
    {
        public IActionResult Index()
        {
            var viewModel = new FeatureAViewModel
            {
                Message = "Feature A message",
                InputText = 123
                //InputText = "ABCD"
            };

            return View("FeatureA", viewModel);
        }
    }
}
