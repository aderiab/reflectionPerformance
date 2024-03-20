using System;
using System.Linq.Expressions;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace ReflectionPerformance;

public class Program
{
    public static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<Performance>();
    }
}

public class Performance
{
    public static readonly Person Person = new() { Name = "John" };
    private static readonly int N = 100000;

    private static void RegularPropertyGet(Person p)
    {
        var sb = new StringBuilder();

        for (var i = 0; i < N; i++)
        {
            sb.AppendLine(p.Name);
        }
    }

    private static void RegularPropertySet(Person p)
    {
        for (var i = 0; i < N; i++)
        {
            p.Name = "Bob" + i;
        }
    }

    private static void ReflectionGet(Person p)
    {
        var sb = new StringBuilder();

        for (var i = 0; i < N; i++)
        {
            var property = p.GetType().GetProperty("Name");

            sb.AppendLine(property?.GetValue(p, null).ToString());
        }
    }

    private static void ReflectionSet(Person p)
    {
        for (var i = 0; i < N; i++)
        {
            var property = p.GetType().GetProperty("Name");
            property?.SetValue(p, "Bob" + i);
        }
    }

    private static void ReflectionWitchCachedPropertyInfoGet(Person p)
    {
        var property = p.GetType().GetProperty("Name");
        var sb = new StringBuilder();
        for (var i = 0; i < N; i++)
        {
            sb.AppendLine(property?.GetValue(p, null).ToString());
        }
    }

    private static void ReflectionWitchCachedPropertyInfoSet(Person p)
    {
        var property = p.GetType().GetProperty("Name");
        for (var i = 0; i < N; i++)
        {
            property?.SetValue(p, "Bob" + i);
        }
    }

    private static void CompiledExpressionGet(Person p)
    {
        var sb = new StringBuilder();

        for (var i = 0; i < N; i++)
        {
            var arg = Expression.Parameter(p.GetType(), "x");
            Expression expr = Expression.Property(arg, "Name");
            var propertyResolver = Expression.Lambda<Func<Person, object>>(expr, arg).Compile();
            sb.AppendLine(propertyResolver(p).ToString());
        }
    }

    private static void CompiledExpressionSet(Person p)
    {
        for (var i = 0; i < N; i++)
        {
            var arg = Expression.Parameter(typeof(Person));
            Expression expr = Expression.Property(arg, "Name");
            var get = Expression.Lambda<Func<Person, string>>(expr, arg);
            var member = get.Body;
            var param = Expression.Parameter(typeof(string), "value");
            var setter = Expression
                .Lambda<Action<Person, string>>(Expression.Assign(member, param), get.Parameters[0], param)
                .Compile();
            setter(p, "Bob" + i);
        }
    }

    private static void CachedCompiledExpressionGet(Person p)
    {
        var sb = new StringBuilder();

        var arg = Expression.Parameter(p.GetType(), "x");
        Expression expr = Expression.Property(arg, "Name");

        var propertyResolver = Expression.Lambda<Func<Person, object>>(expr, arg).Compile();

        for (var i = 0; i < N; i++)
        {
            sb.AppendLine(propertyResolver(p).ToString());
        }
    }

    private static void CachedCompiledExpressionSet(Person p)
    {
        var arg = Expression.Parameter(typeof(Person));
        Expression expr = Expression.Property(arg, "Name");
        var get = Expression.Lambda<Func<Person, string>>(expr, arg);
        var member = get.Body;
        var param = Expression.Parameter(typeof(string), "value");
        var setter = Expression
            .Lambda<Action<Person, string>>(Expression.Assign(member, param), get.Parameters[0], param)
            .Compile();

        for (var i = 0; i < N; i++)
        {
            setter(p, "Bob" + i);
        }
    }

    private static void DelegateGet(Person p)
    {
        for (var i = 0; i < N; i++)
        {
            var sb = new StringBuilder();

            var arg = Expression.Parameter(p.GetType(), "x");
            Expression expr = Expression.Property(arg, "Name");

            var propertyResolver = Expression.Lambda(expr, arg).Compile();

            sb.AppendLine(((Func<Person, object>) propertyResolver)(p).ToString());
        }
    }

    private static void DelegateSet(Person p)
    {
        for (var i = 0; i < N; i++)
        {
            var arg = Expression.Parameter(typeof(Person));
            Expression expr = Expression.Property(arg, "Name");
            var get = Expression.Lambda<Func<Person, string>>(expr, arg);
            var member = get.Body;
            var param = Expression.Parameter(typeof(string), "value");
            var setter = Expression
                .Lambda<Action<Person, string>>(Expression.Assign(member, param), get.Parameters[0], param)
                .Compile();

            ((Action<Person, string>)setter)(p, "Bob" + i);
        }
    }

    private static void CachedDelegateGet(Person p)
    {
        StringBuilder sb = new StringBuilder();

        ParameterExpression arg = Expression.Parameter(p.GetType(), "x");
        Expression expr = Expression.Property(arg, "Name");

        var propertyResolver = Expression.Lambda(expr, arg).Compile();

        for (int i = 0; i < N; i++)
        {
            sb.AppendLine(((Func<Person, object>)propertyResolver)(p).ToString());
        }
    }
    
    private static void CachedDelegateSet(Person p)
    {
        var arg = Expression.Parameter(typeof(Person));
        Expression expr = Expression.Property(arg, "Name");
        var get = Expression.Lambda<Func<Person, string>>(expr, arg);
        var member = get.Body;
        var param = Expression.Parameter(typeof(string), "value");
        var setter = Expression
            .Lambda<Action<Person, string>>(Expression.Assign(member, param), get.Parameters[0], param)
            .Compile();

        for (var i = 0; i < N; i++)
        {
            ((Action<Person, string>)setter)(p, "Bob" + i);
        }
    }

    [Benchmark]
    public void RegularPropertyGet()
    {
        RegularPropertyGet(Person);
    }

    [Benchmark]
    public void RegularPropertySet()
    {
        RegularPropertySet(Person);
    }

    [Benchmark]
    public void ReflectionGet()
    {
        ReflectionGet(Person);
    }

    [Benchmark]
    public void ReflectionSet()
    {
        ReflectionSet(Person);
    }

    [Benchmark]
    public void ReflectionWitchCachedPropertyInfoGet()
    {
        ReflectionWitchCachedPropertyInfoGet(Person);
    }

    [Benchmark]
    public void ReflectionWitchCachedPropertyInfoSet()
    {
        ReflectionWitchCachedPropertyInfoSet(Person);
    }

    [Benchmark]
    public void CompiledExpressionGet()
    {
        CompiledExpressionGet(Person);
    }

    [Benchmark]
    public void CompiledExpressionSet()
    {
        CompiledExpressionSet(Person);
    }

    [Benchmark]
    public void CachedCompiledExpressionGet()
    {
        CachedCompiledExpressionGet(Person);
    }

    [Benchmark]
    public void CachedCompiledExpressionSet()
    {
        CachedCompiledExpressionSet(Person);
    }

    [Benchmark]
    public void DelegateGet()
    {
        DelegateGet(Person);
    }

    [Benchmark]
    public void DelegateSet()
    {
        DelegateSet(Person);
    }

    [Benchmark]
    public void CachedDelegateGet()
    {
        CachedDelegateGet(Person);
    }
    
    [Benchmark]
    public void CachedDelegateSet()
    {
        CachedDelegateSet(Person);
    }
}

public class Person
{
    public string Name { get; set; }
}