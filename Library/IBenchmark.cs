using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Library;

/// <summary>
/// Enables a benchmark to be selected
/// </summary>
public interface IBenchmark {

    public static void SelectBenchmarks(Assembly assembly) {

        var benchmarks = FindBenchmarks(assembly);

        var oldColor = Console.ForegroundColor;
        
        Console.ForegroundColor = ConsoleColor.Cyan;
        
        if (benchmarks.Length == 0) {
            Console.WriteLine("No benchmarks were found within the assembly.");
            Console.ForegroundColor = oldColor;
            return;
        }

        Console.WriteLine($"Benchmarks found: {benchmarks.Length}");
        
        while (true) {

            Console.WriteLine();

            for (int i = 0; i < benchmarks.Length; i++) {
                Console.WriteLine($"{(i + 1)}. {benchmarks[i].Name}");
            }

            Console.WriteLine();

            Console.Write($"Select benchmark to run (or q to quit): ");
            string key = Console.ReadLine()!;

            if (key == "q") {
                break;
            }
            
            if (!int.TryParse(key, out int result) || result > benchmarks.Length) {
                Console.WriteLine("Invalid benchmark, try again.");
                continue;
            }

            result = Math.Max(result, 1);

            BenchmarkRunner.Run(benchmarks[result - 1]);

            Console.WriteLine("Press anything to continue...");
            Console.ReadKey();
        }

        Console.ForegroundColor = oldColor;
    }

    public static Type[] FindBenchmarks(Assembly assembly) {

        var qualifiedTypes = new List<Type>();

        foreach(var type in assembly.GetTypes()) {
            if (type.IsClass && type.IsAssignableTo(typeof(IBenchmark))) {
                qualifiedTypes.Add(type);
            }
        }

        return qualifiedTypes.ToArray();
    }
}