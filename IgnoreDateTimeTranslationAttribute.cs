using System;

namespace ReflectionPerformance;

[AttributeUsage(AttributeTargets.Property)]
public sealed class IgnoreDateTimeTranslationAttribute : Attribute { }