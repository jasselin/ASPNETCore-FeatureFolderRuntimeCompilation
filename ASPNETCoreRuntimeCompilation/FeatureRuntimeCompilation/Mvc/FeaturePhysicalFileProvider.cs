using System.IO;
using System.Linq;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Mvc
{
    public class FeaturePhysicalFileProvider : IFileProvider
    {
        private PhysicalFileProvider _innerProvider;

        public FeaturePhysicalFileProvider(string path)
        {
            _innerProvider = new PhysicalFileProvider(path);
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            return _innerProvider.GetDirectoryContents(subpath);
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            return _innerProvider.GetFileInfo(subpath);
        }

        public IChangeToken Watch(string filter)
        {
            var fileInfo = _innerProvider.GetFileInfo(filter);

            if (fileInfo.Exists && !fileInfo.Name.StartsWith("_"))
            {
                var currentPath = fileInfo.PhysicalPath;
                do
                {
                    currentPath = Path.GetFullPath(Path.Combine(currentPath, ".."));
                }
                while (Directory.GetFiles(Path.Combine(currentPath, ".."), "*.cshtml").Any());

                var watchDir = currentPath.Substring(_innerProvider.Root.Length).Replace("\\", "/");
                return _innerProvider.Watch(watchDir + "/**/*");
            }

            return _innerProvider.Watch(filter);
        }
    }
}