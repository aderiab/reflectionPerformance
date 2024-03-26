using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace ReflectionPerformance;

[SimpleJob(RuntimeMoniker.Net48)]
public class Performance
{
    private Person _person;
    private PropertyInfo _cachedProperty;
    private Func<Person, object> _cachedExpressionGetter;
    private Action<Person, string> _cachedExpressionSetter;
    private Delegate _cachedDelegateGetter;
    private Action<Person, string> _cachedDelegateSetter;
    
    private Action<object, object?> _cachedSetter;
    private Func<object, object> _cachedGetter;

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

        {
            var arg = typeof(Person);
            var property = arg.GetProperties().FirstOrDefault();
            var declaringClass = property.DeclaringType;
            var typeOfResult = property.PropertyType;
            _cachedGetter = ReflectionHelper.Getter(declaringClass, typeOfResult, property.GetMethod);
        }

        {
            var arg = typeof(Person);
            var property = arg.GetProperties().FirstOrDefault();
            var declaringClass = property.DeclaringType;
            var typeOfResult = property.PropertyType;
            _cachedSetter = ReflectionHelper.Setter(declaringClass, typeOfResult, property.SetMethod);
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
        return property?.GetValue(person, null).ToString();
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
    
    [Benchmark]
    public string ReflectionHelperGet()
    {
        var arg = typeof(Person);
        var property = arg.GetProperties().FirstOrDefault();
        var declaringClass = property.DeclaringType;
        var typeOfResult = property.PropertyType;
        var getter = ReflectionHelper.Getter(declaringClass, typeOfResult, property.GetMethod);
        
        return getter(_person).ToString();
    }
    
    [Benchmark]
    public void ReflectionHelperSet()
    {
        var arg = typeof(Person);
        var property = arg.GetProperties().FirstOrDefault();
        var declaringClass = property.DeclaringType;
        var typeOfResult = property.PropertyType;
        var setter = ReflectionHelper.Setter(declaringClass, typeOfResult, property.SetMethod);
        
        setter(_person, "Bob");
    }
    
    [Benchmark]
    public string CachedReflectionHelperGet()
    {
        return _cachedGetter(_person).ToString();
    }
    
    [Benchmark]
    public void CachedReflectionHelperSet()
    {
        _cachedSetter(_person, "Bob");
    }
}