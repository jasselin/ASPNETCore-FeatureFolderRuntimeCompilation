﻿using System.Reflection;

namespace ASPNETCoreRuntimeCompilation.FeatureRuntimeCompilation.Configuration
{
    public class FeatureRuntimeCompilationOptions
    {
        public FeatureRuntimeCompilationOptions(Assembly assembly, string projectPath)
        {
            Assembly = assembly;
            ProjectPath = projectPath;
            AssemblyName = assembly.GetName().Name;
        }

        //TODO: Make this a list
        public Assembly Assembly { get; set; }
        public string AssemblyName { get; set; }
        public string FeatureNamespace => string.Concat(AssemblyName, ".Features");
        public string ProjectPath { get; set; }

        // RazorReferenceManager loads references from a path, not possible at this moment to provide assemblies from memory
        public bool UseInMemoryAssemblies => false; 
    }
}
