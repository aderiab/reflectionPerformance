using System;

namespace ReflectionPerformance;

public class Day
{
    public DateTime MyDate { get; set; }
    
    [IgnoreDateTimeTranslation]
    public DateTime IgnoredDate { get; set; }
}