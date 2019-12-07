using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace FeatureRuntimeCompilation.Caching
{
    public class FeatureChecksumGenerator : IFeatureChecksumGenerator
    {
        public string GetChecksum(FeatureMetadata metadata)
        {
            var sb = new StringBuilder();

            using var md5 = MD5.Create();
            foreach (var filePath in Directory.GetFiles(metadata.FeaturePath, "*.cs", SearchOption.AllDirectories))
            {
                using var stream = File.OpenRead(filePath);
                var hash = md5.ComputeHash(stream);
                var value = BitConverter.ToString(hash).Replace("-", string.Empty);
                sb.Append(value);
            }

            return sb.ToString();
        }
    }
}
