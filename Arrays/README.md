# Array benchmarks

## Table of Contents
- [Sum](#Sum)



### Sum

Description: Sum of integer array

Source: [Sum.cs](https://github.com/AkseliDev/CSharp_Benchmarks/blob/master/Arrays/Sum.cs)

Fastest method: Vectorized_V256_UnrolledAddress
Fastest safe method: For_Unrolled_Multiples
Slowest method: For_Unsafe_RefIndexing

| Ranks               | Name                            |
| ------------------- |:--------------------------------|
| Fastest method      | Vectorized_V256_UnrolledAddress |
| Fastest safe method | For_Unrolled_Multiples          |
| Slowest method      | For_Unsafe_RefIndexing          |


Results:
|                          Method | Size |      Mean |    Error |   StdDev |
|-------------------------------- |----- |----------:|---------:|---------:|
|                             For | 1000 | 323.10 ns | 1.376 ns | 1.149 ns |
|                         ForEach | 1000 | 322.96 ns | 0.838 ns | 0.784 ns |
|                        For_Span | 1000 | 312.13 ns | 0.397 ns | 0.352 ns |
|                            Linq | 1000 | 350.37 ns | 0.752 ns | 0.667 ns |
|                    For_Unrolled | 1000 | 241.77 ns | 0.321 ns | 0.300 ns |
|          For_Unrolled_Multiples | 1000 | 181.88 ns | 0.511 ns | 0.426 ns |
|              For_Unsafe_RefMath | 1000 | 315.41 ns | 0.628 ns | 0.557 ns |
|          For_Unsafe_RefIndexing | 1000 | 488.87 ns | 1.231 ns | 1.151 ns |
|     For_Unsafe_Unrolled_RefMath | 1000 | 239.56 ns | 0.459 ns | 0.430 ns |
| For_Unsafe_Unrolled_RefIndexing | 1000 | 247.81 ns | 0.317 ns | 0.296 ns |
|                      Vectorized | 1000 |  51.04 ns | 0.110 ns | 0.098 ns |
|                 Vectorized_V256 | 1000 |  49.46 ns | 0.540 ns | 0.505 ns |
|        Vectorized_V256_Unrolled | 1000 |  36.94 ns | 0.080 ns | 0.066 ns |
| Vectorized_V256_UnrolledAddress | 1000 |  28.10 ns | 0.061 ns | 0.051 ns |

