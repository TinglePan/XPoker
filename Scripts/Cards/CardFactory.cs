using System;
using System.Reflection;
using XCardGame.Scripts.Common;

namespace XCardGame.Scripts.Cards;

public static class CardFactory
{
    public static BaseCard CreateInstance(string typeName, params object[] args)
    {
        var qualifiedTypeName = $"{MethodBase.GetCurrentMethod()?.DeclaringType?.Namespace}.{typeName}";
        Type type = Type.GetType(qualifiedTypeName);
        if (type != null && typeof(BaseCard).IsAssignableFrom(type))
        {
            return (BaseCard)Activator.CreateInstance(type, args);
        }
        throw new ArgumentException("Type not found or does not implement BaseCard.");
    }
}