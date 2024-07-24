using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace XCardGame;

[Serializable]
public class BaseDef
{
    public class CustomSerializationBinder : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            return Assembly.GetExecutingAssembly().GetType(typeName);
        }
    }
    
    protected static readonly BinaryFormatter Formatter = new ()
    {
        Binder = new CustomSerializationBinder()
    };
    
    public T Clone<T>()
    {
        using (MemoryStream stream = new MemoryStream())
        {
            Formatter.Serialize(stream, this);
            stream.Seek(0, SeekOrigin.Begin);
            return (T)Formatter.Deserialize(stream);
        }
    }
}