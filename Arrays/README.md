# Array benchmarks

## Table of Contents
- [IntSum](#IntSum)
- [ByteSum](#ByteSum)


### IntSum

Description: Sum of integer array

Source: [IntSum.cs](https://github.com/AkseliDev/CSharp_Benchmarks/blob/master/Arrays/IntSum.cs)


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

### ByteSum

Description: Sum of byte array

Source: [ByteSum.cs](https://github.com/AkseliDev/CSharp_Benchmarks/blob/master/Arrays/ByteSum.cs)

| Ranks               | Name                            |
| ------------------- |:--------------------------------|
| Fastest method      | Vectorized_WidenLoad_CustomWiden_Unrolled |
| Fastest safe method | For_Unrolled_Multiples          |
| Slowest method      | For_Unsafe_RefIndexing          |

Results:
|                                    Method | Size |      Mean |    Error |   StdDev |
|------------------------------------------ |----- |----------:|---------:|---------:|
|                                       For | 1000 | 349.49 ns | 6.903 ns | 8.217 ns |
|                                   ForEach | 1000 | 322.73 ns | 0.331 ns | 0.277 ns |
|                                  For_Span | 1000 | 323.75 ns | 0.984 ns | 0.768 ns |
|                              For_Unrolled | 1000 | 269.14 ns | 3.002 ns | 2.808 ns |
|                    For_Unrolled_Multiples | 1000 | 221.71 ns | 0.341 ns | 0.285 ns |
|                        For_Unsafe_RefMath | 1000 | 329.03 ns | 0.185 ns | 0.164 ns |
|                    For_Unsafe_RefIndexing | 1000 | 364.32 ns | 0.467 ns | 0.414 ns |
|               For_Unsafe_Unrolled_RefMath | 1000 | 244.50 ns | 0.318 ns | 0.297 ns |
|           For_Unsafe_Unrolled_RefIndexing | 1000 | 311.19 ns | 0.931 ns | 0.778 ns |
|                        Vectorized_AvxLoad | 1000 |  36.42 ns | 0.116 ns | 0.091 ns |
|                                Vectorized | 1000 |  26.09 ns | 0.065 ns | 0.051 ns |
|                 Vectorized_CustomWidening | 1000 |  25.24 ns | 0.115 ns | 0.096 ns |
|                      Vectorized_WidenLoad | 1000 |  25.70 ns | 0.087 ns | 0.077 ns |
|          Vectorized_WidenLoad_CustomWiden | 1000 |  22.85 ns | 0.052 ns | 0.046 ns |
| Vectorized_WidenLoad_CustomWiden_Unrolled | 1000 |  18.82 ns | 0.031 ns | 0.026 ns |

