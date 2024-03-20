```

BenchmarkDotNet v0.13.12, Windows 11 (10.0.22631.3296/23H2/2023Update/SunValley3)
AMD Ryzen 7 5700U with Radeon Graphics, 1 CPU, 16 logical and 8 physical cores
  [Host]             : .NET Framework 4.8.1 (4.8.9181.0), X64 RyuJIT VectorSize=256
  .NET Framework 4.8 : .NET Framework 4.8.1 (4.8.9181.0), X64 RyuJIT VectorSize=256

Job=.NET Framework 4.8  Runtime=.NET Framework 4.8  

```
| Method                               | Mean           | Error         | StdDev        | Median         |
|------------------------------------- |---------------:|--------------:|--------------:|---------------:|
| RegularPropertyGet                   |      0.0095 ns |     0.0103 ns |     0.0091 ns |      0.0102 ns |
| RegularPropertySet                   |      1.4268 ns |     0.0240 ns |     0.0224 ns |      1.4221 ns |
| ReflectionGet                        |    179.9620 ns |     2.2098 ns |     2.0671 ns |    179.4725 ns |
| ReflectionSet                        |    273.6485 ns |     1.6982 ns |     1.5885 ns |    274.0634 ns |
| ReflectionWitchCachedPropertyInfoGet |    122.0373 ns |     2.4278 ns |     3.0704 ns |    121.7356 ns |
| ReflectionWitchCachedPropertyInfoSet |    200.1801 ns |     0.8094 ns |     0.7571 ns |    200.4093 ns |
| CompiledExpressionGet                | 52,977.6904 ns |   155.5971 ns |   145.5456 ns | 52,966.3879 ns |
| CompiledExpressionSet                | 60,807.9370 ns |   268.7491 ns |   251.3881 ns | 60,762.0483 ns |
| CachedCompiledExpressionGet          |      5.7889 ns |     0.0654 ns |     0.0612 ns |      5.7709 ns |
| CachedCompiledExpressionSet          |      6.2302 ns |     0.0491 ns |     0.0459 ns |      6.2266 ns |
| DelegateGet                          | 60,222.9233 ns | 1,204.2871 ns | 2,171.5780 ns | 59,160.3394 ns |
| DelegateSet                          | 60,952.7196 ns |   248.7329 ns |   220.4952 ns | 60,967.0898 ns |
| CachedDelegateGet                    |     82.5141 ns |     1.0335 ns |     0.9667 ns |     82.2628 ns |
| CachedDelegateSet                    |      6.2114 ns |     0.0302 ns |     0.0282 ns |      6.2066 ns |
