using System;

namespace ReflectionPerformance;

public record CachedProperty(
    string Name,
    Type PropertyType,
    Type? DeclaringType,
    Action<object, object?>? Setter,
    Func<object, object>? Getter,
    object[]? Attributes)
{
    public string Name { get; set; } = Name;

    public Type PropertyType { get; set; } = PropertyType;

    public Type? DeclaringType { get; set; } = DeclaringType;

    public Action<object, object?>? Setter { get; set; } = Setter;

    public Func<object, object>? Getter { get; set; } = Getter;

    public object[]? Attributes { get; set; } = Attributes;
}