using BenchmarkDotNet.Running;

namespace ReflectionPerformance;

public class Program
{
    public static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<Performance>();
    }
}