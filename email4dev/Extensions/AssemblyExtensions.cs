using System;
using System.Reflection;

namespace email4dev.Extensions
{
    public static class AssemblyExtensions
    {
        public static void LoadDependencies(this Assembly resourceAssembly)
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                var resourceName = string.Join(".", new[] { resourceAssembly.GetName().Name, "Resources", new AssemblyName(args.Name).Name, "dll" });

                using (var stream = resourceAssembly.GetManifestResourceStream(resourceName))
                {
                    if (stream != null)
                    {
                        var assemblyData = new Byte[stream.Length];
                        stream.Read(assemblyData, 0, assemblyData.Length);
                        var assembly = Assembly.Load(assemblyData);

                        if (assembly.GetName().FullName == args.Name)
                            return assembly;
                    }
                }

                return null;
            };
        }
    }
}
