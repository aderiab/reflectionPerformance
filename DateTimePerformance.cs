using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace ReflectionPerformance;

[SimpleJob(RuntimeMoniker.Net48)]
public class DateTimePerformance
{
    private Day _day;
    private PropertyInfo _cachedProperty;
    private Func<Day, DateTime> _cachedExpressionGetter;
    private Action<Day, DateTime> _cachedExpressionSetter;
    private Delegate _cachedDelegateGetter;
    private Action<Day, DateTime> _cachedDelegateSetter;
    
    private Action<object, object?> _cachedSetter;
    private Func<object, object> _cachedGetter;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _day = new Day() { MyDate = new DateTime(2024, 3, 26, 14, 40, 44) };
        _cachedProperty = typeof(Day).GetProperty(nameof(Day.MyDate));

        {
            var arg = Expression.Parameter(typeof(Day), "x");
            Expression expr = Expression.Property(arg, nameof(Day.MyDate));
            _cachedExpressionGetter = Expression.Lambda<Func<Day, DateTime>>(expr, arg).Compile();
        }

        {
            var arg = Expression.Parameter(typeof(Day));
            Expression expr = Expression.Property(arg, nameof(Day.MyDate));
            var get = Expression.Lambda<Func<Day, DateTime>>(expr, arg);
            var member = get.Body;
            var param = Expression.Parameter(typeof(DateTime), "value");
            _cachedExpressionSetter = Expression
                .Lambda<Action<Day, DateTime>>(Expression.Assign(member, param), get.Parameters[0], param)
                .Compile();
        }

        {
            var arg = Expression.Parameter(typeof(Day), "x");
            Expression expr = Expression.Property(arg, nameof(Day.MyDate));
            _cachedDelegateGetter = Expression.Lambda(expr, arg).Compile();
        }

        {
            var arg = Expression.Parameter(typeof(Day));
            Expression expr = Expression.Property(arg, nameof(Day.MyDate));
            var get = Expression.Lambda<Func<Day, DateTime>>(expr, arg);
            var member = get.Body;
            var param = Expression.Parameter(typeof(DateTime), "value");
            _cachedDelegateSetter = Expression
                .Lambda<Action<Day, DateTime>>(Expression.Assign(member, param), get.Parameters[0], param)
                .Compile();
        }

        {
            var arg = typeof(Day);
            var property = arg.GetProperties().FirstOrDefault();
            var declaringClass = property.DeclaringType;
            var typeOfResult = property.PropertyType;
            _cachedGetter = ReflectionHelper.Getter(declaringClass, typeOfResult, property.GetMethod);
        }

        {
            var arg = typeof(Day);
            var property = arg.GetProperties().FirstOrDefault();
            var declaringClass = property.DeclaringType;
            var typeOfResult = property.PropertyType;
            _cachedSetter = ReflectionHelper.Setter(declaringClass, typeOfResult, property.SetMethod);
        }
    }

    [Benchmark]
    public DateTime RegularPropertyGet()
    {
        return _day.MyDate;
    }

    [Benchmark]
    public void RegularPropertySet()
    {
        _day.MyDate = new DateTime(2022, 5, 6, 7, 8, 9);
    }

    [Benchmark]
    public object ReflectionGet()
    {
        var Day = _day;
        var property = Day.GetType().GetProperty(nameof(Day.MyDate));
        return property?.GetValue(Day, null);
    }

    [Benchmark]
    public void ReflectionSet()
    {
        var p = _day;
        var property = p.GetType().GetProperty(nameof(Day.MyDate));
        property?.SetValue(p, new DateTime(2022, 3,4,5,6,7,8));
    }

    [Benchmark]
    public object ReflectionWitchCachedPropertyInfoGet()
    {
        return _cachedProperty.GetValue(_day, null);
    }

    [Benchmark]
    public void ReflectionWitchCachedPropertyInfoSet()
    {
        _cachedProperty.SetValue(_day, new DateTime(2022, 3,4,5,6,7,8));
    }

    [Benchmark]
    public object CompiledExpressionGet()
    {
        var arg = Expression.Parameter(typeof(Day), "x");
        Expression expr = Expression.Property(arg, nameof(Day.MyDate));
        var getter = Expression.Lambda<Func<Day, DateTime>>(expr, arg).Compile();

        return getter(_day);
    }

    [Benchmark]
    public void CompiledExpressionSet()
    {
        var arg = Expression.Parameter(typeof(Day));
        Expression expr = Expression.Property(arg, nameof(Day.MyDate));
        var get = Expression.Lambda<Func<Day, DateTime>>(expr, arg);
        var member = get.Body;
        var param = Expression.Parameter(typeof(DateTime), "value");
        var setter = Expression
            .Lambda<Action<Day, DateTime>>(Expression.Assign(member, param), get.Parameters[0], param)
            .Compile();

        setter(_day, new DateTime(2022, 3,4,5,6,7,8));
    }

    [Benchmark]
    public string CachedCompiledExpressionGet()
    {
        return _cachedExpressionGetter(_day).ToString();
    }

    [Benchmark]
    public void CachedCompiledExpressionSet()
    {
        _cachedExpressionSetter(_day, new DateTime(2022, 3,4,5,6,7,8));
    }

    [Benchmark]
    public string DelegateGet()
    {
        var arg = Expression.Parameter(typeof(Day), "x");
        Expression expr = Expression.Property(arg, nameof(Day.MyDate));
        var propertyResolver = Expression.Lambda(expr, arg).Compile();

        return ((Func<Day, DateTime>) propertyResolver)(_day).ToString();
    }

    // it is the same as CompiledExpressionSet =\
    [Benchmark]
    public void DelegateSet()
    {
        var arg = Expression.Parameter(typeof(Day));
        Expression expr = Expression.Property(arg, nameof(Day.MyDate));
        var get = Expression.Lambda<Func<Day, DateTime>>(expr, arg);
        var member = get.Body;
        var param = Expression.Parameter(typeof(DateTime), "value");
        var setter = Expression
            .Lambda<Action<Day, DateTime>>(Expression.Assign(member, param), get.Parameters[0], param)
            .Compile();

        setter(_day, new DateTime(2022, 3,4,5,6,7,8));
    }

    [Benchmark]
    public object CachedDelegateGet()
    {
        return ((Func<Day, DateTime>) _cachedDelegateGetter)(_day);
    }

    [Benchmark]
    public void CachedDelegateSet()
    {
        _cachedDelegateSetter(_day, new DateTime(2022, 3,4,5,6,7,8));
    }
    
    [Benchmark]
    public object ReflectionHelperGet()
    {
        var arg = typeof(Day);
        var property = arg.GetProperties().FirstOrDefault();
        var declaringClass = property.DeclaringType;
        var typeOfResult = property.PropertyType;
        var getter = ReflectionHelper.Getter(declaringClass, typeOfResult, property.GetMethod);
        
        return getter(_day);
    }
    
    [Benchmark]
    public void ReflectionHelperSet()
    {
        var arg = typeof(Day);
        var property = arg.GetProperties().FirstOrDefault();
        var declaringClass = property.DeclaringType;
        var typeOfResult = property.PropertyType;
        var setter = ReflectionHelper.Setter(declaringClass, typeOfResult, property.SetMethod);
        
        setter(_day, new DateTime(2022, 3,4,5,6,7,8));
    }
    
    [Benchmark]
    public object CachedReflectionHelperGet()
    {
        return _cachedGetter(_day);
    }
    
    [Benchmark]
    public void CachedReflectionHelperSet()
    {
        _cachedSetter(_day, new DateTime(2022, 3,4,5,6,7,8));
    }
}