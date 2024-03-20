```

BenchmarkDotNet v0.13.12, Windows 11 (10.0.22631.3296/23H2/2023Update/SunValley3)
AMD Ryzen 7 5700U with Radeon Graphics, 1 CPU, 16 logical and 8 physical cores
  [Host]     : .NET Framework 4.8.1 (4.8.9181.0), X86 LegacyJIT
  DefaultJob : .NET Framework 4.8.1 (4.8.9181.0), X86 LegacyJIT


```
| Method                               | Mean         | Error      | StdDev      |
|------------------------------------- |-------------:|-----------:|------------:|
| RegularPropertyGet                   |     1.556 ms |  0.0244 ms |   0.0217 ms |
| RegularPropertySet                   |     8.374 ms |  0.1672 ms |   0.2651 ms |
| ReflectionGet                        |    20.615 ms |  0.4092 ms |   0.7166 ms |
| ReflectionSet                        |    38.163 ms |  0.7523 ms |   1.0043 ms |
| ReflectionWitchCachedPropertyInfoGet |    13.802 ms |  0.1983 ms |   0.1855 ms |
| ReflectionWitchCachedPropertyInfoSet |    31.025 ms |  0.3409 ms |   0.3022 ms |
| CompiledExpressionGet                | 4,193.458 ms | 46.4240 ms |  43.4250 ms |
| CompiledExpressionSet                | 4,570.293 ms | 31.8571 ms |  29.7992 ms |
| CachedCompiledExpressionGet          |     2.427 ms |  0.0234 ms |   0.0207 ms |
| CachedCompiledExpressionSet          |     9.423 ms |  0.0537 ms |   0.0449 ms |
| DelegateGet                          | 4,741.994 ms | 30.6888 ms |  25.6265 ms |
| DelegateSet                          | 4,986.362 ms | 99.5774 ms | 191.8519 ms |
| CachedDelegateGet                    |    13.061 ms |  0.1236 ms |   0.1032 ms |
| CachedDelegateSet                    |    10.206 ms |  0.1908 ms |   0.2343 ms |
