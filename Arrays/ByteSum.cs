using BenchmarkDotNet.Attributes;
using Iced.Intel;
using Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

public class ByteSum : IBenchmark {

    private byte[] _data;

    [Params(1000)]
    public int Size { get; set; }

    [GlobalSetup]
    public void GlobalSetup() {
        var data = new byte[Size];

        for (int i = 0; i < data.Length; i++) {
            data[i] = (byte)i;
        }

        _data = data;
    }
    /*
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
        foreach (int value in _data) {
            sum += value;
        }
        return sum;
    }

    [Benchmark]
    public int For_Span() {
        Span<byte> data = _data.AsSpan();
        int sum = 0;
        for (int i = 0; i < data.Length; i++) {
            sum += data[i];
        }
        return sum;
    }

    [Benchmark]
    public int For_Unrolled() {
        var data = _data;
        int sum = 0;
        int i;
        for (i = 0; i <= data.Length - 4; i += 4) {
            sum += data[i];
            sum += data[i + 1];
            sum += data[i + 2];
            sum += data[i + 3];
        }
        for (; i < data.Length; i++) {
            sum += data[i];
        }
        return sum;
    }

    [Benchmark]
    public int For_Unrolled_Multiples() {
        var data = _data;
        int sumA = 0;
        int sumB = 0;
        int sumC = 0;
        int sumD = 0;
        int i;
        for (i = 0; i <= data.Length - 4; i += 4) {
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
        ref byte start = ref MemoryMarshal.GetArrayDataReference(data);
        ref byte end = ref Unsafe.Add(ref start, data.Length);

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
        ref byte start = ref MemoryMarshal.GetArrayDataReference(data);

        int sum = 0;
        for (nint i = 0; i < data.Length; i++) {
            sum += Unsafe.Add(ref start, i);
        }
        return sum;
    }

    [Benchmark]
    public int For_Unsafe_Unrolled_RefMath() {

        var data = _data;
        ref byte start = ref MemoryMarshal.GetArrayDataReference(data);
        ref byte end = ref Unsafe.Add(ref start, data.Length);

        int sum = 0;
        while (Unsafe.ByteOffset(ref start, ref end) >= 4 * sizeof(byte)) {
            sum += start;
            sum += Unsafe.Add(ref start, 1);
            sum += Unsafe.Add(ref start, 2);
            sum += Unsafe.Add(ref start, 3);
            start = ref Unsafe.Add(ref start, 4);
        }
        while (Unsafe.IsAddressLessThan(ref start, ref end)) {
            sum += start;
            start = ref Unsafe.Add(ref start, 1);
        }
        return sum;
    }

    [Benchmark]
    public int For_Unsafe_Unrolled_RefIndexing() {

        var data = _data;
        ref byte start = ref MemoryMarshal.GetArrayDataReference(data);

        int sum = 0;
        nint i;
        for (i = 0; i <= data.Length - 4; i += 4) {
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

    // This algorithm uses VPMOVZXBW to load V256<ushort> from bytes
    // The sum is then calculated using ushorts until an overflow is possible
    // and after the vector is widened to int vectors and added to the final sum
    [Benchmark]
    public unsafe int Vectorized_AvxLoad() {
        var data = _data;

        Vector256<int> sum = Vector256<int>.Zero;

        fixed (byte* p = data) {

            var ptr = p;
            byte* end = p + data.Length;

            if (data.Length >= Vector256<short>.Count) {

                byte* vEnd = end - Vector256<short>.Count - 1;

                do {
                    Vector256<short> offload = Vector256<short>.Zero;

                    int index = 0;

                    while (index < 128 && ptr < vEnd) {
                        index++;
                        offload += Avx2.ConvertToVector256Int16(ptr);
                        ptr += Vector256<short>.Count;
                    }

                    (Vector256<int> low, Vector256<int> high) = Vector256.Widen(offload);
                    sum += low;
                    sum += high;
                } while (ptr < vEnd);
            }

            int result = Vector256.Sum(sum);
            while (ptr < end) {
                result += *ptr;
                ptr++;
            }
            return result;
        }
    }

    // This algorithm loads a V256<ushort> 2 times with a 1 byte offset
    // then shaves off the low/high bytes of the ushorts
    // the sum is calculated using ushorts until an overflow is possible
    // and after that widened to int vectors and then added to the final sum 
    [Benchmark]
    public int Vectorized() {
        var data = _data;
        ref byte ptr = ref MemoryMarshal.GetArrayDataReference(data);
        ref byte end = ref Unsafe.Add(ref ptr, data.Length);

        Vector256<int> sum = Vector256<int>.Zero;
        Vector256<ushort> mask = Vector256.Create((ushort)0xff);

        if (data.Length >= Vector256<byte>.Count + 1) {

            ref byte vEnd = ref Unsafe.Subtract(ref end, Vector256<byte>.Count - 1);

            do {
                Vector256<ushort> offload = Vector256<ushort>.Zero;

                int index = 0;
                while (index < 128 && Unsafe.IsAddressLessThan(ref ptr, ref vEnd)) {
                    index++;
                    offload += Vector256.LoadUnsafe(ref ptr).AsUInt16() & mask;
                    offload += Vector256.LoadUnsafe(ref Unsafe.Add(ref ptr, 1)).AsUInt16() & mask;
                    ptr = ref Unsafe.Add(ref ptr, Vector256<byte>.Count);
                }

                (Vector256<uint> low, Vector256<uint> high) = Vector256.Widen(offload);
                sum += low.AsInt32();
                sum += high.AsInt32();
            } while (Unsafe.IsAddressLessThan(ref ptr, ref vEnd));
        }

        int result = Vector256.Sum(sum);
        while (Unsafe.IsAddressLessThan(ref ptr, ref end)) {
            result += ptr;
            ptr = ref Unsafe.Add(ref ptr, 1);
        }
        return result;
    }

    // The algorithm is same as Vectorized except using a handrolled widening technique instead of using Vector256.Widen
    [Benchmark]
    public int Vectorized_CustomWidening() {
        var data = _data;
        ref byte ptr = ref MemoryMarshal.GetArrayDataReference(data);
        ref byte end = ref Unsafe.Add(ref ptr, data.Length);

        Vector256<int> sum = Vector256<int>.Zero;
        Vector256<ushort> mask = Vector256.Create((ushort)0xff);
        Vector256<uint> widenMask = Vector256.Create((uint)ushort.MaxValue);

        if (data.Length >= Vector256<byte>.Count + 1) {

            ref byte vEnd = ref Unsafe.Subtract(ref end, Vector256<byte>.Count - 1);

            do {
                Vector256<ushort> offload = Vector256<ushort>.Zero;

                int index = 0;
                while (index < 128 && Unsafe.IsAddressLessThan(ref ptr, ref vEnd)) {
                    index++;
                    offload += Vector256.LoadUnsafe(ref ptr).AsUInt16() & mask;
                    offload += Vector256.LoadUnsafe(ref Unsafe.Add(ref ptr, 1)).AsUInt16() & mask;
                    ptr = ref Unsafe.Add(ref ptr, Vector256<byte>.Count);
                }

                sum += (offload.AsUInt32() & widenMask).AsInt32();
                sum += Vector256.ShiftRightLogical(offload.AsUInt32(), 16).AsInt32();

            } while (Unsafe.IsAddressLessThan(ref ptr, ref vEnd));
        }

        int result = Vector256.Sum(sum);
        while (Unsafe.IsAddressLessThan(ref ptr, ref end)) {
            result += ptr;
            ptr = ref Unsafe.Add(ref ptr, 1);
        }
        return result;
    }

    // The algorithm loads a byte vector and then widens it to ushort vectors
    // the sum is calculated using ushorts until an overflow is possible
    // and after that widened to int vectors and then added to the final sum 
    [Benchmark]
    public int Vectorized_WidenLoad() {
        var data = _data;
        ref byte ptr = ref MemoryMarshal.GetArrayDataReference(data);
        ref byte end = ref Unsafe.Add(ref ptr, data.Length);

        Vector256<int> sum = Vector256<int>.Zero;

        if (data.Length >= Vector256<byte>.Count) {

            ref byte vEnd = ref Unsafe.Subtract(ref end, Vector256<byte>.Count - 1);

            do {
                Vector256<ushort> offload = Vector256<ushort>.Zero;

                int index = 0;
                while (index < 128 && Unsafe.IsAddressLessThan(ref ptr, ref vEnd)) {
                    index++;
                    (Vector256<ushort> a, Vector256<ushort> b) = Vector256.Widen(Vector256.LoadUnsafe(ref ptr));
                    offload += a;
                    offload += b;
                    ptr = ref Unsafe.Add(ref ptr, Vector256<byte>.Count);
                }

                (Vector256<uint> low, Vector256<uint> high) = Vector256.Widen(offload);
                sum += low.AsInt32();
                sum += high.AsInt32();
            } while (Unsafe.IsAddressLessThan(ref ptr, ref vEnd));
        }

        int result = Vector256.Sum(sum);
        while (Unsafe.IsAddressLessThan(ref ptr, ref end)) {
            result += ptr;
            ptr = ref Unsafe.Add(ref ptr, 1);
        }
        return result;
    }

    // the algorithm is same as Vectorized_WidenLoad except using a handrolled widening technique
    [Benchmark]
    public int Vectorized_WidenLoad_CustomWiden() {
        var data = _data;
        ref byte ptr = ref MemoryMarshal.GetArrayDataReference(data);
        ref byte end = ref Unsafe.Add(ref ptr, data.Length);

        Vector256<int> sum = Vector256<int>.Zero;
        Vector256<ushort> mask = Vector256.Create((ushort)0xff);
        Vector256<uint> widenMask = Vector256.Create((uint)ushort.MaxValue);

        if (data.Length >= Vector256<byte>.Count) {

            ref byte vEnd = ref Unsafe.Subtract(ref end, Vector256<byte>.Count - 1);

            do {
                Vector256<ushort> offload = Vector256<ushort>.Zero;

                int index = 0;
                while (index < 128 && Unsafe.IsAddressLessThan(ref ptr, ref vEnd)) {
                    index++;
                    Vector256<ushort> vec = Vector256.LoadUnsafe(ref ptr).AsUInt16();
                    offload += vec & mask;
                    offload += Vector256.ShiftRightLogical(vec, 8);
                    ptr = ref Unsafe.Add(ref ptr, Vector256<byte>.Count);
                }

                sum += (offload.AsUInt32() & widenMask).AsInt32();
                sum += Vector256.ShiftRightLogical(offload.AsUInt32(), 16).AsInt32();

            } while (Unsafe.IsAddressLessThan(ref ptr, ref vEnd));
        }

        int result = Vector256.Sum(sum);
        while (Unsafe.IsAddressLessThan(ref ptr, ref end)) {
            result += ptr;
            ptr = ref Unsafe.Add(ref ptr, 1);
        }
        return result;
    }
    */
    // same as Vectorized_WidenLoad_CustomWiden but unrolled for maximum performance
    [Benchmark]
    public int Vectorized_WidenLoad_CustomWiden_Unrolled() {
        var data = _data;
        ref byte ptr = ref MemoryMarshal.GetArrayDataReference(data);
        ref byte end = ref Unsafe.Add(ref ptr, data.Length);

        Vector256<int> sum = Vector256<int>.Zero;
        Vector256<ushort> mask = Vector256.Create((ushort)0xff);
        Vector256<uint> widenMask = Vector256.Create((uint)ushort.MaxValue);

        if (data.Length >= Vector256<byte>.Count * 4) {

            ref byte vEnd = ref Unsafe.Subtract(ref end, Vector256<byte>.Count - 1);

            do {

                Vector256<ushort> offload = Vector256<ushort>.Zero;

                int index = 0;
                while (index < 128 && Unsafe.IsAddressLessThan(ref ptr, ref Unsafe.Subtract(ref end, Vector256<byte>.Count * 4 - 1))) {

                    index += 4;

                    Vector256<ushort> vec = Vector256.LoadUnsafe(ref ptr).AsUInt16();
                    offload += vec & mask;
                    offload += Vector256.ShiftRightLogical(vec, 8);

                    vec = Vector256.LoadUnsafe(ref Unsafe.Add(ref ptr, Vector256<byte>.Count)).AsUInt16();
                    offload += vec & mask;
                    offload += Vector256.ShiftRightLogical(vec, 8);

                    vec = Vector256.LoadUnsafe(ref Unsafe.Add(ref ptr, Vector256<byte>.Count * 2)).AsUInt16();
                    offload += vec & mask;
                    offload += Vector256.ShiftRightLogical(vec, 8);

                    vec = Vector256.LoadUnsafe(ref Unsafe.Add(ref ptr, Vector256<byte>.Count * 3)).AsUInt16();
                    offload += vec & mask;
                    offload += Vector256.ShiftRightLogical(vec, 8);

                    ptr = ref Unsafe.Add(ref ptr, Vector256<byte>.Count * 4);
                }

                while (index < 128 && Unsafe.IsAddressLessThan(ref ptr, ref vEnd)) {
                    index++;
                    Vector256<ushort> vec = Vector256.LoadUnsafe(ref ptr).AsUInt16();
                    offload += vec & mask;
                    offload += Vector256.ShiftRightLogical(vec, 8);
                    ptr = ref Unsafe.Add(ref ptr, Vector256<byte>.Count);
                }

                sum += (offload.AsUInt32() & widenMask).AsInt32();
                sum += Vector256.ShiftRightLogical(offload.AsUInt32(), 16).AsInt32();

            } while (Unsafe.IsAddressLessThan(ref ptr, ref vEnd));

        } else if (data.Length >= Vector256<byte>.Count) {

            ref byte vEnd = ref Unsafe.Subtract(ref end, Vector256<byte>.Count - 1);

            do {
                Vector256<ushort> offload = Vector256<ushort>.Zero;

                int index = 0;
                while (index < 128 && Unsafe.IsAddressLessThan(ref ptr, ref vEnd)) {
                    index++;
                    Vector256<ushort> vec = Vector256.LoadUnsafe(ref ptr).AsUInt16();
                    offload += vec & mask;
                    offload += Vector256.ShiftRightLogical(vec, 8);
                    ptr = ref Unsafe.Add(ref ptr, Vector256<byte>.Count);
                }

                sum += (offload.AsUInt32() & widenMask).AsInt32();
                sum += Vector256.ShiftRightLogical(offload.AsUInt32(), 16).AsInt32();

            } while (Unsafe.IsAddressLessThan(ref ptr, ref vEnd));
        }

        int result = Vector256.Sum(sum);
        while (Unsafe.IsAddressLessThan(ref ptr, ref end)) {
            result += ptr;
            ptr = ref Unsafe.Add(ref ptr, 1);
        }
        return result;
    }

    [Benchmark]
    public int Vectorized_WidenLoad_CustomWiden_Unrolled_Indexing() {
        var data = _data;
        ref byte ptr = ref MemoryMarshal.GetArrayDataReference(data);
        Vector256<int> sum = Vector256<int>.Zero;
        Vector256<ushort> mask = Vector256.Create((ushort)0xff);
        Vector256<uint> widenMask = Vector256.Create((uint)ushort.MaxValue);
        nint i = 0;
        if (data.Length >= Vector256<byte>.Count * 4) {
            do {
                Vector256<ushort> offload = Vector256<ushort>.Zero;
                int index = 0;
                while (index < 128 && i <= data.Length - Vector256<byte>.Count * 4) {

                    Vector256<ushort> vec = Vector256.LoadUnsafe(ref Unsafe.Add(ref ptr, i)).AsUInt16();
                    offload += vec & mask;
                    offload += Vector256.ShiftRightLogical(vec, 8);

                    vec = Vector256.LoadUnsafe(ref Unsafe.Add(ref ptr, i + Vector256<byte>.Count)).AsUInt16();
                    offload += vec & mask;
                    offload += Vector256.ShiftRightLogical(vec, 8);

                    vec = Vector256.LoadUnsafe(ref Unsafe.Add(ref ptr, i + Vector256<byte>.Count * 2)).AsUInt16();
                    offload += vec & mask;
                    offload += Vector256.ShiftRightLogical(vec, 8);

                    vec = Vector256.LoadUnsafe(ref Unsafe.Add(ref ptr, i + Vector256<byte>.Count * 3)).AsUInt16();
                    offload += vec & mask;
                    offload += Vector256.ShiftRightLogical(vec, 8);

                    index += 4;
                    i += Vector256<byte>.Count * 4;

                }
                while (index < 128 && i <= data.Length - Vector256<byte>.Count) {
                    index++;
                    Vector256<ushort> vec = Vector256.LoadUnsafe(ref Unsafe.Add(ref ptr, i)).AsUInt16();
                    offload += vec & mask;
                    offload += Vector256.ShiftRightLogical(vec, 8);
                    i += Vector256<byte>.Count;
                }
                sum += (offload.AsUInt32() & widenMask).AsInt32();
                sum += Vector256.ShiftRightLogical(offload.AsUInt32(), 16).AsInt32();

            } while (i <= data.Length - Vector256<byte>.Count);
        } else if (data.Length >= Vector256<byte>.Count) {
            do {
                Vector256<ushort> offload = Vector256<ushort>.Zero;
                int index = 0;
                while (index < 128 && i <= data.Length - Vector256<byte>.Count) {
                    index++;
                    Vector256<ushort> vec = Vector256.LoadUnsafe(ref Unsafe.Add(ref ptr, i)).AsUInt16();
                    offload += vec & mask;
                    offload += Vector256.ShiftRightLogical(vec, 8);
                    i += Vector256<byte>.Count;
                }
            } while (i <= data.Length - Vector256<byte>.Count);
        }
        int result = Vector256.Sum(sum);
        for (; i < data.Length; i++) {
            result += Unsafe.Add(ref ptr, i);
        }
        return result;
    }
}