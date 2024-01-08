using BenchmarkDotNet.Attributes;
using Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Text;
using System.Threading.Tasks;

public class EvenNumbers : IBenchmark {

    private int[] _data;

    [Params(10_000, 100_000)]
    public int Size { get; set; }

    [GlobalSetup]
    public void GlobalSetup() {
        var data = new int[Size];
        var random = new Random(Size);
        for (int i = 0; i < data.Length; i++) {
            data[i] = random.Next(1, 1024);
        }

        _data = data;
    }

    [Benchmark]
    public int CountEvenNumbers() {
        int count = 0;
        for(int i = 0; i < _data.Length; i++) {
            if (_data[i] % 2 == 0) {
                count++;
            }
        }
        return count;
    }

    [Benchmark]
    public int CountEvenNumbers_Local() {
        var data = _data;
        int count = 0;
        for (int i = 0; i < data.Length; i++) {
            if (data[i] % 2 == 0) {
                count++;
            }
        }
        return count;
    }

    [Benchmark]
    public int CountEvenNumbers_Branchless() {
        var data = _data;
        int count = 0;
        for (int i = 0; i < data.Length; i++) {
            count += ~data[i] & 1;
        }
        return count;
    }

    [Benchmark]
    public int CountEvenNumbers_BranchlessUsingOdds() {
        var data = _data;
        int count = 0;
        for (int i = 0; i < data.Length; i++) {
            count += data[i] & 1;
        }
        // count odd numbers and substract from total to get even numbers
        return data.Length - count;
    }

    [Benchmark]
    public int CountEvenNumbers_Unrolled() {
        var data = _data;
        int count = 0;
        int i = 0;
        for(; i <= data.Length - 4; i += 4) {
            count += data[i] & 1;
            count += data[i + 1] & 1;
            count += data[i + 2] & 1;
            count += data[i + 3] & 1;
        }
        for(; i < data.Length; i++) {
            count += data[i] & 1;
        }
        return data.Length - count;
    }

    [Benchmark]
    public int CountEvenNumbers_SafeVectorized() {
        var array = _data;
        Vector<int> vsum = Vector<int>.Zero;
        int i = 0;
        for(; i <= array.Length - Vector<int>.Count; i += Vector<int>.Count) {
            vsum += new Vector<int>(array, i) & new Vector<int>(1);
        }
        int count = Vector.Sum(vsum);
        for(; i < array.Length; i++) {
            count += array[i] & 1;
        }
        return array.Length - count;
    }

    [Benchmark]
    public int CountEvenNumbers_Vectorized() {
        var array = _data;
        ref int start = ref MemoryMarshal.GetArrayDataReference(array);
        nint i = 0;
        Vector256<int> vsum = Vector256<int>.Zero;
        for (; i <= array.Length - Vector256<int>.Count; i += Vector256<int>.Count) {
            vsum += Vector256.LoadUnsafe(ref start, (nuint)i) & Vector256.Create(1);
        }
        int count = Vector256.Sum(vsum);
        for (; i < array.Length; i++) {
            count += array[i] & 1;
        }
        return array.Length - count;
    }

    [Benchmark]
    public int CountEvenNumbers_VectorizedUnrolled() {
        var array = _data;
        ref int start = ref MemoryMarshal.GetArrayDataReference(array);
        nint i = 0;
        Vector256<int> vsum = Vector256<int>.Zero;

        for (; i <= array.Length - Vector256<int>.Count * 4; i += Vector256<int>.Count * 4) {
            vsum += Vector256.LoadUnsafe(ref start, (nuint)i) & Vector256.Create(1);
            vsum += Vector256.LoadUnsafe(ref start, (nuint)(i + Vector256<int>.Count)) & Vector256.Create(1);
            vsum += Vector256.LoadUnsafe(ref start, (nuint)(i + Vector256<int>.Count * 2)) & Vector256.Create(1);
            vsum += Vector256.LoadUnsafe(ref start, (nuint)(i + Vector256<int>.Count * 3)) & Vector256.Create(1);
        }
        if (i <= array.Length - Vector256<int>.Count * 2) {
            vsum += Vector256.LoadUnsafe(ref start, (nuint)i) & Vector256.Create(1);
            vsum += Vector256.LoadUnsafe(ref start, (nuint)(i + Vector256<int>.Count)) & Vector256.Create(1);
            i += Vector256<int>.Count * 2;
        }
        if (i <= array.Length - Vector256<int>.Count) {
            vsum += Vector256.LoadUnsafe(ref start, (nuint)i) & Vector256.Create(1);
            i += Vector256<int>.Count;
        }
        int count = Vector256.Sum(vsum);
        for (; i < array.Length; i++) {
            count += array[i] & 1;
        }
        return array.Length - count;
    }
}