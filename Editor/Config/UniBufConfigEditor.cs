using ProtoBuf.Meta;
using UnityEditor;
using UnityEngine;

namespace UniBuf.EditorExt
{
    [CustomEditor(typeof(UniBufConfig))]
    public class UniBufConfigEditor : Editor
    {
        private UniBufConfig _config;
        private void OnEnable()
        {
            _config = (UniBufConfig) target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Create proto file"))
            {
                UniBufHelper.GenerateProtoFile(GetTypeModel(), _config.ProtoFilePath);
            }

            if (GUILayout.Button("Compile model"))
            {
                UniBufHelper.CompileModel(GetTypeModel());
            }
        }

        private RuntimeTypeModel GetTypeModel()
        {
            return TypeModelFactory.CreateTypeModel(UnityAssembliesUtils.GetAllRuntimeTypes(false));
        }
    }
}