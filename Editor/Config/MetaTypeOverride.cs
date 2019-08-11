using ProtoBuf.Meta;
using UnityEngine;

namespace UniBuf.EditorExt
{
    public abstract class MetaTypeOverride : ScriptableObject
    {
        public abstract void RegisterMetaType(MetaType type);
    }
}