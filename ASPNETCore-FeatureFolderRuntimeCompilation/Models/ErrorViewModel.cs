using System;

namespace ASPNETCore_FeatureFolderRuntimeCompilation.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
