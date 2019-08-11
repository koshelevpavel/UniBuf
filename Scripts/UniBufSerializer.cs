using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;
using ProtoBuf;
using ProtoBuf.Meta;

namespace UniBuf
{
    public static class UniBufSerializer
    {
        public const string TYPE_MODEL_NAME = "UniBufTypeModel";

        // Inject in editor, TypeModelEditorInjector
        private static TypeModel _typeModel;
        public static TypeModel TypeModel => _typeModel;

#if !UNITY_EDITOR
    static UniBufSerializer()
    {
        _typeModel = new UniBufTypeModel();
    }
#endif
        public static T DeepClone<T>(T instance)
        {
            if (instance != null)
                return (T) TypeModel.DeepClone(instance);
            return instance;
        }

        public static T Merge<T>(Stream source, T instance)
        {
            return (T) TypeModel.Deserialize(source, instance, typeof(T));
        }

        public static T Deserialize<T>(Stream source)
        {
            return (T) TypeModel.Deserialize(source, null, typeof(T));
        }

        public static void Serialize<T>(Stream destination, T instance)
        {
            if (instance == null)
                return;
            TypeModel.Serialize(destination, instance);
        }

        public static TTo ChangeType<TFrom, TTo>(TFrom instance)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                Serialize(memoryStream, instance);
                memoryStream.Position = 0L;
                return Deserialize<TTo>(memoryStream);
            }
        }

        public static void Serialize<T>(SerializationInfo info, T instance) where T : class, ISerializable
        {
            Serialize(info, new StreamingContext(StreamingContextStates.Persistence), instance);
        }

        public static void Serialize<T>(SerializationInfo info, StreamingContext context, T instance)
            where T : class, ISerializable
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));
            if (instance.GetType() != typeof(T))
                throw new ArgumentException("Incorrect type", nameof(instance));
            using (MemoryStream memoryStream = new MemoryStream())
            {
                TypeModel.Serialize(memoryStream, instance, context);
                info.AddValue("proto", memoryStream.ToArray());
            }
        }

        public static void Serialize<T>(XmlWriter writer, T instance) where T : IXmlSerializable
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));
            using (MemoryStream memoryStream = new MemoryStream())
            {
                Serialize(memoryStream, instance);
                writer.WriteBase64(memoryStream.GetBuffer(), 0, (int) memoryStream.Length);
            }
        }

        public static void Merge<T>(XmlReader reader, T instance) where T : IXmlSerializable
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));
            byte[] buffer = new byte[4096];
            using (MemoryStream memoryStream = new MemoryStream())
            {
                int depth = reader.Depth;
                while (reader.Read() && reader.Depth > depth)
                {
                    if (reader.NodeType == XmlNodeType.Text)
                    {
                        int count;
                        while ((count = reader.ReadContentAsBase64(buffer, 0, 4096)) > 0)
                            memoryStream.Write(buffer, 0, count);
                        if (reader.Depth <= depth)
                            break;
                    }
                }

                memoryStream.Position = 0L;
                Merge(memoryStream, instance);
            }
        }

        public static void Merge<T>(SerializationInfo info, T instance) where T : class, ISerializable
        {
            Merge(info, new StreamingContext(StreamingContextStates.Persistence), instance);
        }

        public static void Merge<T>(SerializationInfo info, StreamingContext context, T instance)
            where T : class, ISerializable
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));
            if (instance.GetType() != (object) typeof(T))
                throw new ArgumentException("Incorrect type", nameof(instance));
            using (MemoryStream memoryStream = new MemoryStream((byte[]) info.GetValue("proto", typeof(byte[]))))
            {
                if (!ReferenceEquals((T) TypeModel.Deserialize(memoryStream, instance, typeof(T), context), instance))
                    throw new ProtoException("Deserialization changed the instance; cannot succeed.");
            }
        }

        public static IFormatter CreateFormatter<T>()
        {
            return TypeModel.CreateFormatter(typeof(T));
        }

        public static IEnumerable<T> DeserializeItems<T>(
            Stream source,
            PrefixStyle style,
            int fieldNumber)
        {
            return TypeModel.DeserializeItems<T>(source, style, fieldNumber);
        }

        public static bool TryReadLengthPrefix(Stream source, PrefixStyle style, out int length)
        {
            int fieldNumber;
            int bytesRead;
            length = ProtoReader.ReadLengthPrefix(source, false, style, out fieldNumber, out bytesRead);
            return bytesRead > 0;
        }

        public static bool TryReadLengthPrefix(
            byte[] buffer,
            int index,
            int count,
            PrefixStyle style,
            out int length)
        {
            using (Stream source = new MemoryStream(buffer, index, count))
                return TryReadLengthPrefix(source, style, out length);
        }

        public static class NonGeneric
        {
            public static object DeepClone(object instance)
            {
                if (instance != null)
                    return TypeModel.DeepClone(instance);
                return null;
            }

            public static void Serialize(Stream dest, object instance)
            {
                if (instance == null)
                    return;
                TypeModel.Serialize(dest, instance);
            }

            public static object Deserialize(Type type, Stream source)
            {
                return TypeModel.Deserialize(source, null, type);
            }

            public static object Merge(Stream source, object instance)
            {
                if (instance == null)
                    throw new ArgumentNullException(nameof(instance));
                return TypeModel.Deserialize(source, instance, instance.GetType(), null);
            }

            public static bool CanSerialize(Type type)
            {
                return TypeModel.IsDefined(type);
            }
        }
    }
}