using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace UniBuf.EditorExt
{
    internal class UniBufPreprocessBuild : IPreprocessBuildWithReport
    {
        public int callbackOrder => 100;

        public void OnPreprocessBuild(BuildReport report)
        {
            UniBufConfig config = UniBufHelper.GetConfig();
            bool isTest =
                (report.summary.options & BuildOptions.IncludeTestAssemblies) == BuildOptions.IncludeTestAssemblies;

            var typeModel = TypeModelFactory.CreateTypeModel(UnityAssembliesUtils.GetAllRuntimeTypes(isTest));

            if (config.AutoCompileModel)
            {
                UniBufHelper.CompileModel(typeModel);
            }

            if (config.AutoGenerateProtoFile)
            {
                UniBufHelper.GenerateProtoFile(typeModel, config.ProtoFilePath);
            }
        }
    }
}
