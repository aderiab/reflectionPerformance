#nullable enable
using System;
using System.Reflection;

namespace ReflectionPerformance;

public class ReflectionHelper
{
    private static readonly MethodInfo? CallInnerSetDelegateMethod =
        typeof(ReflectionHelper).GetMethod(nameof(CallInnerSetDelegate), BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? CallInnerGetDelegateMethod =
        typeof(ReflectionHelper).GetMethod(nameof(CallInnerGetDelegate), BindingFlags.NonPublic | BindingFlags.Static);

    public static Func<object, object>? Getter(Type declaringClass, Type typeOfResult, MethodInfo getMethod)
    {
        var getMethodDelegateType = typeof(Func<,>).MakeGenericType(declaringClass, typeOfResult);
        var getMethodDelegate = getMethod.CreateDelegate(getMethodDelegateType);
        var callGetInnerGenericMethodWithTypes = CallInnerGetDelegateMethod?
            .MakeGenericMethod(declaringClass, typeOfResult);
        return (Func<object, object>?) callGetInnerGenericMethodWithTypes?.Invoke(null,
            new object[] { getMethodDelegate });
    }

    public static Action<object, object?>? Setter(Type declaringClass, Type typeOfResult, MethodInfo setMethod)
    {
        var setMethodDelegateType = typeof(Action<,>).MakeGenericType(declaringClass, typeOfResult);
        var setMethodDelegate = setMethod.CreateDelegate(setMethodDelegateType);
        var callSetInnerGenericMethodWithTypes = CallInnerSetDelegateMethod?
            .MakeGenericMethod(declaringClass, typeOfResult);
        return (Action<object, object?>?) callSetInnerGenericMethodWithTypes?.Invoke(null,
            new object[] { setMethodDelegate });
    }

    private static Action<object, object> CallInnerSetDelegate<TClass, TProperty>(
        Action<TClass?, TProperty?> @delegate)
    {
        return (instance, property) => @delegate((TClass?) instance, (TProperty?) property);
    }

    private static Func<object, object?> CallInnerGetDelegate<TClass, TResult>(
        Func<TClass?, TResult> @delegate)
    {
        return instance => @delegate((TClass?) instance);
    }
}