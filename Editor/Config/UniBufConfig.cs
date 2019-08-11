using System.IO;
using UnityEngine;

namespace UniBuf.EditorExt
{
    public class UniBufConfig : ScriptableObject
    {
        [SerializeField] private bool _autoCompileModel = true;
        [SerializeField] private bool _autoGenerateProtoFile = true;
        [SerializeField] private string _protoFilePath = Path.Combine(UniBufHelper.UNI_BUF_FOLDER, "type_model.proto");
        
        [SerializeField] private MetaTypeOverride[] _typeOverride = new MetaTypeOverride[0];

        public bool AutoCompileModel => _autoCompileModel;
        public bool AutoGenerateProtoFile => _autoGenerateProtoFile;
        public string ProtoFilePath => _protoFilePath;
        public MetaTypeOverride[] TypeOverride
        {
            get { return _typeOverride; }
            internal set { _typeOverride = value; }
        }
    }
}