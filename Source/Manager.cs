using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace EmbeddedLibraryResolver
{
    public static class Manager
    {
        /// <summary>
        /// Automatically load all embedded libraries.
        /// </summary>
        public static void Load()
        {
            String extension = GetNativeLibraryExtension();

            Assembly assembly = Assembly.GetEntryAssembly()!;

            String[] resourceNames = assembly.GetManifestResourceNames();
            String dllPath = Path.GetTempPath();

            foreach (String resourceName in resourceNames)
            {
                if (!resourceName.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
                    continue;

                String fileName = ExtractFileName(resourceName);
                if (fileName.Length == 0) continue;

                String filePath = Path.Combine(dllPath, fileName);
                ExtractAndLoad(assembly, resourceName, filePath);
            }
        }

        private static void ExtractAndLoad(Assembly assembly, String resourceName, String filePath)
        {
            if (!File.Exists(filePath))
            {
                using Stream? stream = assembly.GetManifestResourceStream(resourceName);
                if (stream == null) return;

                using FileStream output = File.Create(filePath);
                stream.CopyTo(output);
            }

            NativeLibrary.Load(filePath);
        }

        private static String ExtractFileName(String resourceName)
        {
            String[] segments = resourceName.Split('.');
            Int32 count = segments.Length;
            if (count < 2) return String.Empty;
            return segments[count - 2] + "." + segments[count - 1];
        }

        private static String GetNativeLibraryExtension()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return ".dll";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) return ".so";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) return ".dylib";
            throw new PlatformNotSupportedException("Unsupported library extension.");
        }
    }
}
