using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Library;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;

public class IntSum : IBenchmark {

    private int[] _data;

    [Params(1000)]
    public int Size { get; set; }

    [GlobalSetup]
    public void GlobalSetup() {
        _data = Enumerable.Range(0, Size).ToArray();
    }

    [Benchmark]
    public int For() {
        var data = _data;
        int sum = 0;
        for (int i = 0; i < data.Length; i++) {
            sum += data[i];
        }
        return sum;
    }
    
    [Benchmark]
    public int ForEach() {
        int sum = 0;
        foreach(int num in _data) {
            sum += num;
        }
        return sum;
    }

    [Benchmark]
    public int For_Span() {
        Span<int> data = _data.AsSpan();
        int sum = 0;
        for(int i = 0; i < data.Length; i++) {
            sum += data[i];
        }
        return sum;
    }

    [Benchmark]
    public int Linq() {
        return _data.Sum();
    }

    [Benchmark]
    public int For_Unrolled() {
        var data = _data;
        int sum = 0;
        int i;
        for(i = 0; i < data.Length - 4; i += 4) {
            sum += data[i];
            sum += data[i + 1];
            sum += data[i + 2];
            sum += data[i + 3];
        }
        for(; i < data.Length; i++) {
            sum += data[i];
        }
        return sum;
    }

    // super scalar abuse
    [Benchmark]
    public int For_Unrolled_Multiples() {
        var data = _data;
        int sumA = 0;
        int sumB = 0;
        int sumC = 0;
        int sumD = 0;
        int i;
        for (i = 0; i < data.Length - 4; i += 4) {
            sumA += data[i];
            sumB += data[i + 1];
            sumC += data[i + 2];
            sumD += data[i + 3];
        }
        int totalSum = sumA + sumB + sumC + sumD;
        for (; i < data.Length; i++) {
            totalSum += data[i];
        }
        return totalSum;
    }

    [Benchmark]
    public int For_Unsafe_RefMath() {

        var data = _data;

        ref int start = ref MemoryMarshal.GetArrayDataReference(data);
        ref int end = ref Unsafe.Add(ref start, data.Length);
        
        int sum = 0;
        
        while (Unsafe.IsAddressLessThan(ref start, ref end)) {
            sum += start;
            start = ref Unsafe.Add(ref start, 1);
        }

        return sum;
    }

    [Benchmark]
    public int For_Unsafe_RefIndexing() {

        var data = _data;
        ref int start = ref MemoryMarshal.GetArrayDataReference(data);

        int sum = 0;
        for(int i = 0; i < data.Length; i++) {
            sum += Unsafe.Add(ref start, i);
        }
        return sum;
    }

    [Benchmark]
    public int For_Unsafe_Unrolled_RefMath() {

        var data = _data;
        ref int start = ref MemoryMarshal.GetArrayDataReference(data);
        ref int end = ref Unsafe.Add(ref start, data.Length);

        int sum = 0;
        while(Unsafe.ByteOffset(ref start, ref end) >= 4 * sizeof(int)) {
            sum += start;
            sum += Unsafe.Add(ref start, 1);
            sum += Unsafe.Add(ref start, 2);
            sum += Unsafe.Add(ref start, 3);
            start = ref Unsafe.Add(ref start, 4);
        }
        while(Unsafe.IsAddressLessThan(ref start, ref end)) {
            sum += start;
            start = ref Unsafe.Add(ref start, 1);
        }
        return sum;
    }

    [Benchmark]
    public int For_Unsafe_Unrolled_RefIndexing() {

        var data = _data;
        ref int start = ref MemoryMarshal.GetArrayDataReference(data);

        int sum = 0;
        int i;
        for(i = 0; i < data.Length - 4; i += 4) {
            sum += Unsafe.Add(ref start, i);
            sum += Unsafe.Add(ref start, i + 1);
            sum += Unsafe.Add(ref start, i + 2);
            sum += Unsafe.Add(ref start, i + 3);
        }
        for (; i < data.Length; i++) {
            sum += Unsafe.Add(ref start, i);
        }
        return sum;
    }

    [Benchmark]
    public int Vectorized() {
        var data = _data;
        Vector<int> sum = Vector<int>.Zero;
        int i;
        for(i = 0; i < data.Length - Vector<int>.Count; i += Vector<int>.Count) {
            sum += new Vector<int>(data, i);
        }
        int result = Vector.Sum(sum);
        for(; i < data.Length; i++) {
            result += data[i];
        }
        return result;
    }
    
    [Benchmark]
    public int Vectorized_V256() {
        var data = _data;
        Vector256<int> sum = Vector256<int>.Zero;
        int i;
        for (i = 0; i < data.Length - Vector256<int>.Count; i += Vector256<int>.Count) {
            sum += Vector256.LoadUnsafe(ref data[i]);
        }
        int result = Vector256.Sum(sum);
        for (; i < data.Length; i++) {
            result += data[i];
        }
        return result;
    }
    [Benchmark]
    public int Vectorized_V256_Unrolled() {
        var data = _data;
        ref int r0 = ref MemoryMarshal.GetArrayDataReference(data);
        Vector256<int> sum = Vector256<int>.Zero;
        int i = 0;
        while (data.Length - i >= Vector256<int>.Count * 8) {
            sum += Vector256.LoadUnsafe(ref Unsafe.Add(ref r0, i));
            sum += Vector256.LoadUnsafe(ref Unsafe.Add(ref r0, i + Vector256<int>.Count));
            sum += Vector256.LoadUnsafe(ref Unsafe.Add(ref r0, i + Vector256<int>.Count * 2));
            sum += Vector256.LoadUnsafe(ref Unsafe.Add(ref r0, i + Vector256<int>.Count * 3));
            sum += Vector256.LoadUnsafe(ref Unsafe.Add(ref r0, i + Vector256<int>.Count * 4));
            sum += Vector256.LoadUnsafe(ref Unsafe.Add(ref r0, i + Vector256<int>.Count * 5));
            sum += Vector256.LoadUnsafe(ref Unsafe.Add(ref r0, i + Vector256<int>.Count * 6));
            sum += Vector256.LoadUnsafe(ref Unsafe.Add(ref r0, i + Vector256<int>.Count * 7));
            i += Vector256<int>.Count * 8;
        }
        for (; i < data.Length - Vector256<int>.Count; i += Vector256<int>.Count) {
            sum += Vector256.LoadUnsafe(ref Unsafe.Add(ref r0, i));
        }
        int result = Vector256.Sum(sum);
        for (; i < data.Length; i++) {
            result += data[i];
        }
        return result;
    }

    [Benchmark]
    public int Vectorized_V256_UnrolledAddress() {
        var data = _data;
        ref int r0 = ref MemoryMarshal.GetArrayDataReference(data);
        ref int end = ref Unsafe.Add(ref r0, data.Length);
        Vector256<int> sum = Vector256<int>.Zero;

        if (data.Length >= Vector256<int>.Count * 8) {
            while (Unsafe.IsAddressLessThan(ref r0, ref Unsafe.Subtract(ref end, Vector256<int>.Count * 8))) {
                sum += Vector256.LoadUnsafe(ref r0);
                sum += Vector256.LoadUnsafe(ref Unsafe.Add(ref r0, Vector256<int>.Count));
                sum += Vector256.LoadUnsafe(ref Unsafe.Add(ref r0, Vector256<int>.Count * 2));
                sum += Vector256.LoadUnsafe(ref Unsafe.Add(ref r0, Vector256<int>.Count * 3));
                sum += Vector256.LoadUnsafe(ref Unsafe.Add(ref r0, Vector256<int>.Count * 4));
                sum += Vector256.LoadUnsafe(ref Unsafe.Add(ref r0, Vector256<int>.Count * 5));
                sum += Vector256.LoadUnsafe(ref Unsafe.Add(ref r0, Vector256<int>.Count * 6));
                sum += Vector256.LoadUnsafe(ref Unsafe.Add(ref r0, Vector256<int>.Count * 7));
                r0 = ref Unsafe.Add(ref r0, Vector256<int>.Count * 8);
            }

            if (Unsafe.IsAddressLessThan(ref r0, ref Unsafe.Subtract(ref end, Vector256<int>.Count * 4))) {
                sum += Vector256.LoadUnsafe(ref r0);
                sum += Vector256.LoadUnsafe(ref Unsafe.Add(ref r0, Vector256<int>.Count));
                sum += Vector256.LoadUnsafe(ref Unsafe.Add(ref r0, Vector256<int>.Count * 2));
                sum += Vector256.LoadUnsafe(ref Unsafe.Add(ref r0, Vector256<int>.Count * 3));
                r0 = ref Unsafe.Add(ref r0, Vector256<int>.Count * 4);
            }
            if (Unsafe.IsAddressLessThan(ref r0, ref Unsafe.Subtract(ref end, Vector256<int>.Count * 2))) {
                sum += Vector256.LoadUnsafe(ref r0);
                sum += Vector256.LoadUnsafe(ref Unsafe.Add(ref r0, Vector256<int>.Count));
                r0 = ref Unsafe.Add(ref r0, Vector256<int>.Count * 2);
            }
        }

        int result = Vector256.Sum(sum);

        while (Unsafe.IsAddressLessThan(ref r0, ref end)) {
            result += r0;
            r0 = ref Unsafe.Add(ref r0, 1);
        }
        return result;
    }
}