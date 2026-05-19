using BenchmarkDotNet.Running;

namespace DbLocalizationProvider.Benchmarks;

public static class Program
{
    public static void Main(string[] args)
    {
        if (args.Length > 0 && args[0] == "--heap")
        {
            var n = args.Length > 1 && int.TryParse(args[1], out var parsed) ? parsed : 10_000;
            HeapRetentionMeasurement.Run(n);
            return;
        }

        BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
    }
}
