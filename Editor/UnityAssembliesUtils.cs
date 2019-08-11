using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Compilation;

namespace UniBuf.EditorExt
{
    public static class UnityAssembliesUtils
    {
        public static IEnumerable<Type> GetAllEditorTypes()
        {
            return GetTypes(CompilationPipeline.GetAssemblies(AssembliesType.Editor));
        }

        public static IEnumerable<Type> GetAllRuntimeTypes(bool isIncludeTestTypes)
        {
            if (isIncludeTestTypes)
            {
                return GetTypes(CompilationPipeline.GetAssemblies(AssembliesType.Editor).Where(
                    assembly => (assembly.flags & AssemblyFlags.EditorAssembly) != AssemblyFlags.EditorAssembly));
            }
            return GetTypes(CompilationPipeline.GetAssemblies(AssembliesType.Player));
        }

        private static IEnumerable<Type> GetTypes(IEnumerable<Assembly> assemblies)
        {
            foreach (Assembly assembly in assemblies)
            {
                foreach (Type type in AppDomain.CurrentDomain.Load(assembly.name).GetTypes())
                {
                    yield return type;
                }
            }
        }
    }
}