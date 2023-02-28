using BenchmarkDotNet.Running;
using Library;
using System.Reflection;

IBenchmark.SelectBenchmarks(Assembly.GetExecutingAssembly());