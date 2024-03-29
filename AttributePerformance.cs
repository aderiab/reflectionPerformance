using System;
using System.Linq;
using System.Reflection;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace ReflectionPerformance;

[SimpleJob(RuntimeMoniker.Net48)]
public class AttributePerformance
{
    private object[] _attributes;
    [GlobalSetup]
    public void GlobalSetup()
    {
        {
            var property = typeof(Day).GetProperties().First(p => p.Name == nameof(Day.IgnoredDate));
            _attributes = property.GetAttributes<object>();
        }
    }
    
    [Benchmark]
    public object[] GetAttributes()
    {
        var property = typeof(Day).GetProperties().First(p => p.Name == nameof(Day.IgnoredDate));
        return property.GetAttributes<object>();
    }
    
    [Benchmark]
    public object[] GetAttributesCached()
    {
        return _attributes;
    }
    
}

public static class AttributesUtil
{
    public static T[] GetAttributes<T>(this ICustomAttributeProvider member) where T : class
    {
        if (typeof (T) != typeof (object))
        {
            return (T[]) member.GetCustomAttributes(typeof (T), false);
        }

        return (T[]) member.GetCustomAttributes(false);
    }

    public static bool HasAttribute<T>(this ICustomAttributeProvider member) where T : class
    {
        return member.GetAttributes<T>().FirstOrDefault() != null;
    }
}