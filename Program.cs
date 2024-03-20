using System;
using System.Linq.Expressions;
using System.Reflection;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;

namespace ReflectionPerformance;

public class Program
{
    public static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<Performance>();
    }
}

[SimpleJob(RuntimeMoniker.Net48)]
public class Performance
{
    private Person _person;
    private PropertyInfo _cachedProperty;
    private Func<Person, object> _cachedExpressionGetter;
    private Action<Person, string> _cachedExpressionSetter;
    private Delegate _cachedDelegateGetter;
    private Action<Person, string> _cachedDelegateSetter;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _person = new() { Name = "John" };
        _cachedProperty = typeof(Person).GetProperty(nameof(Person.Name));

        {
            var arg = Expression.Parameter(typeof(Person), "x");
            Expression expr = Expression.Property(arg, nameof(Person.Name));
            _cachedExpressionGetter = Expression.Lambda<Func<Person, object>>(expr, arg).Compile();
        }

        {
            var arg = Expression.Parameter(typeof(Person));
            Expression expr = Expression.Property(arg, nameof(Person.Name));
            var get = Expression.Lambda<Func<Person, string>>(expr, arg);
            var member = get.Body;
            var param = Expression.Parameter(typeof(string), "value");
            _cachedExpressionSetter = Expression
                .Lambda<Action<Person, string>>(Expression.Assign(member, param), get.Parameters[0], param)
                .Compile();
        }

        {
            var arg = Expression.Parameter(typeof(Person), "x");
            Expression expr = Expression.Property(arg, nameof(Person.Name));
            _cachedDelegateGetter = Expression.Lambda(expr, arg).Compile();
        }

        {
            var arg = Expression.Parameter(typeof(Person));
            Expression expr = Expression.Property(arg, nameof(Person.Name));
            var get = Expression.Lambda<Func<Person, string>>(expr, arg);
            var member = get.Body;
            var param = Expression.Parameter(typeof(string), "value");
            _cachedDelegateSetter = Expression
                .Lambda<Action<Person, string>>(Expression.Assign(member, param), get.Parameters[0], param)
                .Compile();
        }
    }

    [Benchmark]
    public string RegularPropertyGet()
    {
        return _person.Name;
    }

    [Benchmark]
    public void RegularPropertySet()
    {
        _person.Name = "Bob";
    }

    [Benchmark]
    public string ReflectionGet()
    {
        var person = _person;
        var property = person.GetType().GetProperty(nameof(Person.Name));
        return property.GetValue(person, null).ToString();
    }

    [Benchmark]
    public void ReflectionSet()
    {
        var p = _person;
        var property = p.GetType().GetProperty(nameof(Person.Name));
        property?.SetValue(p, "Bob");
    }

    [Benchmark]
    public string ReflectionWitchCachedPropertyInfoGet()
    {
        return _cachedProperty.GetValue(_person, null).ToString();
    }

    [Benchmark]
    public void ReflectionWitchCachedPropertyInfoSet()
    {
        _cachedProperty.SetValue(_person, "Bob");
    }

    [Benchmark]
    public string CompiledExpressionGet()
    {
        var arg = Expression.Parameter(typeof(Person), "x");
        Expression expr = Expression.Property(arg, nameof(Person.Name));
        var getter = Expression.Lambda<Func<Person, object>>(expr, arg).Compile();

        return getter(_person).ToString();
    }

    [Benchmark]
    public void CompiledExpressionSet()
    {
        var arg = Expression.Parameter(typeof(Person));
        Expression expr = Expression.Property(arg, nameof(Person.Name));
        var get = Expression.Lambda<Func<Person, string>>(expr, arg);
        var member = get.Body;
        var param = Expression.Parameter(typeof(string), "value");
        var setter = Expression
            .Lambda<Action<Person, string>>(Expression.Assign(member, param), get.Parameters[0], param)
            .Compile();

        setter(_person, "Bob");
    }

    [Benchmark]
    public string CachedCompiledExpressionGet()
    {
        return _cachedExpressionGetter(_person).ToString();
    }

    [Benchmark]
    public void CachedCompiledExpressionSet()
    {
        _cachedExpressionSetter(_person, "Bob");
    }

    [Benchmark]
    public string DelegateGet()
    {
        var arg = Expression.Parameter(typeof(Person), "x");
        Expression expr = Expression.Property(arg, nameof(Person.Name));
        var propertyResolver = Expression.Lambda(expr, arg).Compile();

        return ((Func<Person, object>) propertyResolver)(_person).ToString();
    }

    // it is the same as CompiledExpressionSet =\
    [Benchmark]
    public void DelegateSet()
    {
        var arg = Expression.Parameter(typeof(Person));
        Expression expr = Expression.Property(arg, nameof(Person.Name));
        var get = Expression.Lambda<Func<Person, string>>(expr, arg);
        var member = get.Body;
        var param = Expression.Parameter(typeof(string), "value");
        var setter = Expression
            .Lambda<Action<Person, string>>(Expression.Assign(member, param), get.Parameters[0], param)
            .Compile();

        setter(_person, "Bob");
    }

    [Benchmark]
    public string CachedDelegateGet()
    {
        return ((Func<Person, object>) _cachedDelegateGetter)(_person).ToString();
    }

    [Benchmark]
    public void CachedDelegateSet()
    {
        _cachedDelegateSetter(_person, "Bob");
    }
}

public class Person
{
    public string Name { get; set; }
}