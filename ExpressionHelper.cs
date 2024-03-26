using System;
using System.Linq;
using System.Linq.Expressions;

namespace ReflectionPerformance;

using LinqExpression = Expression;

public static class ExpressionHelper
{
    public static Func<T, object> GetterExpressionTree<T>(T myClass)
    {
        var property = typeof(T).GetProperties().FirstOrDefault();
        var eventLogCustomType = property.DeclaringType;

        var instance = LinqExpression.Parameter(typeof(T));

        Func<T, object> getter = null;
        var getMethod = property.GetGetMethod();
        if (getMethod != null)
        {
            getter =
                LinqExpression.Lambda<Func<T, object>>(
                        LinqExpression.Convert(
                            LinqExpression.Call(
                                LinqExpression.Convert(instance, eventLogCustomType),
                                getMethod),
                            typeof(object)),
                        instance)
                    .Compile();
        }
        return getter;
    }

    public static Action<T, object> SetterExpressionTree<T>(T myClass)
    {
        var property = typeof(T).GetProperties().FirstOrDefault();
        var eventLogCustomType = property.DeclaringType;
        var propertyType = property.PropertyType;

        var instance = LinqExpression.Parameter(typeof(T));
        Action<T, object> setter = null;
        var setMethod = property.GetSetMethod();
        if (setMethod != null)
        {
            var parameter = LinqExpression.Parameter(typeof(object));
            setter =
                LinqExpression.Lambda<Action<T, object>>(
                        LinqExpression.Call(
                            LinqExpression.Convert(instance, eventLogCustomType),
                            setMethod,
                            LinqExpression.Convert(parameter, propertyType)),
                        instance, parameter)
                    .Compile();
        }
        return setter;
    }
}