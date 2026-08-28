using System;
using System.Collections.Generic;

public static class ServiceLocator
{
    private static readonly Dictionary<Type, object> map = new();

    public static void Register<T>(T instance) where T : class
    {
        map[typeof(T)] = instance;
    }

    public static void Register<TInterface, TImpl>(TImpl instance)
        where TImpl : class, TInterface
        where TInterface : class
    {
        map[typeof(TInterface)] = instance;
    }

    public static T Get<T>() where T : class
    {
        if (map.TryGetValue(typeof(T), out object service))
            return (T)service;

        throw new Exception($"Service {typeof(T).Name} not registered. Check Bootstrap order.");
    }

    public static bool TryGet<T>(out T service) where T : class
    {
        if (map.TryGetValue(typeof(T), out object obj))
        {
            service = (T)obj;
            return true;
        }

        service = null;
        return false;
    }

    public static void Reset()
    {
        map.Clear();
    }
}
