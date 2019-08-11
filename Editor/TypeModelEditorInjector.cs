using System;
using System.Reflection;
using UnityEngine;

namespace UniBuf.EditorExt
{
    internal static class TypeModelEditorInjector
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void BeforeSceneLoad()
        {
            var type = typeof(UniBufSerializer);
            var property = type.GetField("_typeModel", BindingFlags.NonPublic | BindingFlags.Static);
            if(property == null)
                throw new Exception($"Not found property \"TypeModel\" in {type}");
            var typeModel = TypeModelFactory.CreateTypeModel(UnityAssembliesUtils.GetAllEditorTypes());
            property.SetValue(null, typeModel);
        }
    }
}