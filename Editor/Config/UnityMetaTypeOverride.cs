using ProtoBuf.Meta;
using UnityEngine;

namespace UniBuf.EditorExt
{
    public class UnityMetaTypeOverride : MetaTypeOverride
    {
        public override void RegisterMetaType(MetaType type)
        {
            if (type.Type == typeof(Vector2)) type.Add("x", "y");
            if (type.Type == typeof(Vector3)) type.Add("x", "y", "z");
            if (type.Type == typeof(Vector4)) type.Add("x", "y", "z", "w");
            if (type.Type == typeof(Quaternion)) type.Add("x", "y", "z", "w");
            if (type.Type == typeof(Color)) type.Add("r", "g", "b", "a");
        }
    }
}