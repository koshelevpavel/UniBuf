using System.IO;
using ProtoBuf.Meta;
using UnityEditor;
using UnityEngine;

namespace UniBuf.EditorExt
{
    public static class UniBufHelper
    {
        public const string UNI_BUF_FOLDER = "Assets/Plugins/UniBuf";
        private const string ConfigName = "UniBufConfig.asset";
        private const string UnityOverrideName = "UniMetaTypeOverride.asset";
        private static string ConfigPath => Path.Combine(UNI_BUF_FOLDER, ConfigName);
        private static string LibraryFullName => $"{UniBufSerializer.TYPE_MODEL_NAME}.dll";
        private static string LibraryPath => Path.Combine(UNI_BUF_FOLDER, LibraryFullName);

        public static UniBufConfig GetConfig()
        {
            UniBufConfig config = AssetDatabase.LoadAssetAtPath<UniBufConfig>(ConfigPath);

            if (config == null)
            {
                config = ScriptableObject.CreateInstance<UniBufConfig>();

                UnityMetaTypeOverride metaTypeOverride = ScriptableObject.CreateInstance<UnityMetaTypeOverride>();

                config.TypeOverride = new MetaTypeOverride[] {metaTypeOverride};

                AssetDatabase.CreateAsset(metaTypeOverride, Path.Combine(UNI_BUF_FOLDER, UnityOverrideName));
                AssetDatabase.CreateAsset(config, ConfigPath);

                AssetDatabase.SaveAssets();
            }

            return config;
        }
        
        
        public static void GenerateProtoFile(RuntimeTypeModel model, string path)
        {
            using (StreamWriter writer = File.CreateText(path))
            {
                writer.Write(model.GetSchema(null));
            }
        }
        
        
        public static void CompileModel(RuntimeTypeModel model)
        {
            model.Compile(UniBufSerializer.TYPE_MODEL_NAME, LibraryFullName);

            if (!Directory.Exists(UNI_BUF_FOLDER))
            {
                Directory.CreateDirectory(UNI_BUF_FOLDER);
            }

            if (File.Exists(LibraryPath))
            {
                File.Delete(LibraryPath);
            }

            File.Move(LibraryFullName, LibraryPath);
            AssetDatabase.ImportAsset(LibraryPath);
        }
    }
}