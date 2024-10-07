```

BenchmarkDotNet v0.14.0, Windows 10 (10.0.19045.4894/22H2/2022Update)
AMD Ryzen 5 5500U with Radeon Graphics, 1 CPU, 12 logical and 6 physical cores
.NET SDK 8.0.303
  [Host]     : .NET 8.0.7 (8.0.724.31311), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.7 (8.0.724.31311), X64 RyuJIT AVX2


```
| Method                | Mean     | Error    | StdDev   | Allocated |
|---------------------- |---------:|---------:|---------:|----------:|
| ElementInsertIntoFull | 65.97 ns | 0.218 ns | 0.194 ns |     488 B |
| Dictionary            | 24.97 ns | 0.074 ns | 0.069 ns |     192 B |
