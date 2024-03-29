# Array benchmarks

## Table of Contents
- [IntSum](#IntSum)
- [ByteSum](#ByteSum)
- [EvenNumbers](#EvenNumbers)


### IntSum

Description: Sum of integer array

Source: [IntSum.cs](https://github.com/AkseliDev/CSharp_Benchmarks/blob/master/Arrays/IntSum.cs)


| Ranks               | Name                            |
| ------------------- |:--------------------------------|
| Fastest method      | Vectorized_V256_UnrolledAddress |
| Fastest safe method | For_Unrolled_Multiples          |
| Slowest method      | Linq                            |


Results:
|                          Method | Size |      Mean |    Error |   StdDev |
|-------------------------------- |----- |----------:|---------:|---------:|
|                             For | 1000 | 326.71 ns | 1.387 ns | 1.083 ns |
|                         ForEach | 1000 | 323.42 ns | 0.549 ns | 0.459 ns |
|                        For_Span | 1000 | 314.62 ns | 0.504 ns | 0.472 ns |
|                            Linq | 1000 | 352.83 ns | 1.953 ns | 1.731 ns |
|                    For_Unrolled | 1000 | 244.54 ns | 0.541 ns | 0.451 ns |
|          For_Unrolled_Multiples | 1000 | 187.34 ns | 0.364 ns | 0.323 ns |
|              For_Unsafe_RefMath | 1000 | 317.36 ns | 0.531 ns | 0.471 ns |
|          For_Unsafe_RefIndexing | 1000 | 317.34 ns | 1.135 ns | 0.948 ns |
|     For_Unsafe_Unrolled_RefMath | 1000 | 240.06 ns | 0.411 ns | 0.343 ns |
| For_Unsafe_Unrolled_RefIndexing | 1000 | 239.24 ns | 0.464 ns | 0.412 ns |
|                      Vectorized | 1000 |  51.10 ns | 0.107 ns | 0.100 ns |
|                 Vectorized_V256 | 1000 |  49.68 ns | 0.312 ns | 0.277 ns |
|        Vectorized_V256_Unrolled | 1000 |  32.59 ns | 0.167 ns | 0.148 ns |
| Vectorized_V256_UnrolledAddress | 1000 |  28.17 ns | 0.044 ns | 0.039 ns |
| Vectorized_V256_UnrolledIndexing | 1000 |  28.14 ns | 0.044 ns | 0.039 ns |

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
|                                       For | 1000 | 319.79 ns | 0.597 ns | 0.529 ns |
|                                   ForEach | 1000 | 320.20 ns | 0.952 ns | 0.844 ns |
|                                  For_Span | 1000 | 320.60 ns | 0.840 ns | 0.656 ns |
|                              For_Unrolled | 1000 | 264.79 ns | 1.517 ns | 1.419 ns |
|                    For_Unrolled_Multiples | 1000 | 220.50 ns | 0.734 ns | 0.687 ns |
|                        For_Unsafe_RefMath | 1000 | 341.15 ns | 1.506 ns | 1.335 ns |
|                    For_Unsafe_RefIndexing | 1000 | 362.79 ns | 1.387 ns | 1.297 ns |
|               For_Unsafe_Unrolled_RefMath | 1000 | 243.09 ns | 0.415 ns | 0.368 ns |
|           For_Unsafe_Unrolled_RefIndexing | 1000 | 308.49 ns | 0.397 ns | 0.331 ns |
|                        Vectorized_AvxLoad | 1000 |  34.71 ns | 0.077 ns | 0.069 ns |
|                                Vectorized | 1000 |  26.20 ns | 0.079 ns | 0.074 ns |
|                 Vectorized_CustomWidening | 1000 |  25.11 ns | 0.063 ns | 0.056 ns |
|                      Vectorized_WidenLoad | 1000 |  26.04 ns | 0.041 ns | 0.036 ns |
|          Vectorized_WidenLoad_CustomWiden | 1000 |  23.13 ns | 0.128 ns | 0.113 ns |
| Vectorized_WidenLoad_CustomWiden_Unrolled | 1000 |  18.67 ns | 0.028 ns | 0.025 ns |
| Vectorized_WidenLoad_CustomWiden_Unrolled_Indexing | 1000 | 18.69 ns | 0.055 ns | 0.046 ns |

### EvenNumbers

Description: Counting even numbers in an array

Source: [EvenNumbers.cs](https://github.com/AkseliDev/CSharp_Benchmarks/blob/master/Arrays/EvenNumbers.cs)

| Ranks               | Name                                |
| ------------------- |:------------------------------------|
| Fastest method      | CountEvenNumbers_VectorizedUnrolled |
| Fastest safe method | CountEvenNumbers_Unrolled           |
| Slowest method      | CountEvenNumbers                    |

Results:
|                               Method |   Size |         Mean |       Error |      StdDev |
|------------------------------------- |------- |-------------:|------------:|------------:|
|                     CountEvenNumbers |  10000 |  32,291.4 ns |    92.37 ns |    81.89 ns |
|               CountEvenNumbers_Local |  10000 |  30,300.2 ns |   124.25 ns |   110.15 ns |
|          CountEvenNumbers_Branchless |  10000 |   4,857.7 ns |    13.92 ns |    13.02 ns |
| CountEvenNumbers_BranchlessUsingOdds |  10000 |   4,040.2 ns |    26.05 ns |    21.75 ns |
|            CountEvenNumbers_Unrolled |  10000 |   2,760.7 ns |     5.91 ns |     4.93 ns |
|      CountEvenNumbers_SafeVectorized |  10000 |     978.6 ns |     3.65 ns |     3.42 ns |
|          CountEvenNumbers_Vectorized |  10000 |     647.2 ns |     1.65 ns |     1.55 ns |
|  CountEvenNumbers_VectorizedUnrolled |  10000 |     356.9 ns |     1.96 ns |     1.83 ns |
|                     CountEvenNumbers | 100000 | 352,125.4 ns | 1,103.18 ns | 1,031.91 ns |
|               CountEvenNumbers_Local | 100000 | 344,026.5 ns |   469.27 ns |   438.95 ns |
|          CountEvenNumbers_Branchless | 100000 |  49,109.3 ns |   151.85 ns |   118.55 ns |
| CountEvenNumbers_BranchlessUsingOdds | 100000 |  39,392.9 ns |   423.23 ns |   395.89 ns |
|            CountEvenNumbers_Unrolled | 100000 |  27,494.9 ns |    70.48 ns |    58.86 ns |
|      CountEvenNumbers_SafeVectorized | 100000 |   9,926.9 ns |    37.58 ns |    35.15 ns |
|          CountEvenNumbers_Vectorized | 100000 |   6,410.3 ns |    36.08 ns |    33.75 ns |
|  CountEvenNumbers_VectorizedUnrolled | 100000 |   3,635.3 ns |    26.42 ns |    22.06 ns |