using System.Collections.Generic;
using Godot;

namespace XCardGame.Common;

public class ResourceCache
{
    public static ResourceCache Instance { get; } = new ResourceCache();
    
    private readonly Dictionary<string, Resource> _cache;
    
    private ResourceCache()
    {
        _cache = new Dictionary<string, Resource>();
    }

    public T Load<T>(string path) where T: Resource
    {
        if (_cache.TryGetValue(path, out var value))
        {
            return value as T;
        }
        var resource = GD.Load<T>(path);
        _cache[path] = resource;
        return resource;
    }
}