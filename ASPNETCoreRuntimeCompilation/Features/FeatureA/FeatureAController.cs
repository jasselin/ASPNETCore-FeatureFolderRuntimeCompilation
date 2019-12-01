using Microsoft.AspNetCore.Mvc;

namespace ASPNETCoreRuntimeCompilation.Features.FeatureA
{
    public class FeatureAController : Controller
    {
        private readonly IFeatureADependency _dependency;

        public FeatureAController(IFeatureADependency dependency)
        {
            _dependency = dependency;
        }

        public IActionResult Index()
        {
            var viewModel = new FeatureAViewModel
            {
                Message = _dependency.GetMessage(),
                InputText = 123
                //InputText = "ABCD"
            };

            return View("FeatureA", viewModel);
        }
    }
}
