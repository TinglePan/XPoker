using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;

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
    
    public T Clone<T>() where T: BaseDef
    {
        var options = new JsonSerializerOptions
        {
            IncludeFields = true,
        };
        var json = JsonSerializer.Serialize(this as T, options);
        return JsonSerializer.Deserialize<T>(json, options);
    }
}