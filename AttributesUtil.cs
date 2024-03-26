using System.Linq;
using System.Reflection;

namespace ReflectionPerformance;

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