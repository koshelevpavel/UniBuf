using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ProtoBuf;
using ProtoBuf.Meta;
using UnityEngine;

namespace UniBuf.EditorExt
{
    public static class TypeModelFactory
    {
        public static RuntimeTypeModel CreateTypeModel(IEnumerable<Type> types)
        {
            RuntimeTypeModel typeModel = TypeModel.Create();

            foreach (var t in types)
            {
                var contract = t.GetCustomAttributes(typeof(ProtoContractAttribute), false);
                if (contract.Length > 0)
                {
                    MetaType metaType = typeModel.Add(t, true);

                    // support ISerializationCallbackReceiver
                    if (typeof(ISerializationCallbackReceiver).IsAssignableFrom(t))
                    {
                        MethodInfo beforeSerializeMethod = t.GetMethod("OnBeforeSerialize");
                        MethodInfo afterDeserializeMethod = t.GetMethod("OnAfterDeserialize");

                        metaType.SetCallbacks(beforeSerializeMethod, null, null, afterDeserializeMethod);
                    }
                }
            }

            UniBufConfig config = UniBufHelper.GetConfig();
            HashSet<Type> hashSet = new HashSet<Type>();

            foreach (MetaType metaType in typeModel.GetTypes().Cast<MetaType>().ToArray())
            {
                hashSet.Add(metaType.Type);
                OverrideMetaType(metaType, config.TypeOverride);
                foreach (ValueMember valueMember in metaType.GetFields())
                {
                    if(valueMember.MemberType.IsPrimitive || valueMember.MemberType == typeof(string)) continue;
                    if (!hashSet.Contains(valueMember.MemberType))
                    {
                        MetaType memberType = typeModel.Add(valueMember.MemberType, false);
                        hashSet.Add(memberType.Type);
                        OverrideMetaType(memberType, config.TypeOverride);
                    }
                }
            }

            return typeModel;
        }

        private static void OverrideMetaType(MetaType metaType, IEnumerable<MetaTypeOverride> overrides)
        {
            foreach (MetaTypeOverride typeOverride in overrides)
            {
                typeOverride.RegisterMetaType(metaType);
            }
        }
    }
}