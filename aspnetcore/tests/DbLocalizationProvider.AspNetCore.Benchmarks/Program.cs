using BenchmarkDotNet.Running;
using DbLocalizationProvider.AspNetCore.Tests.ClientsideProvider;

BenchmarkRunner.Run<RequestHandlerBenchmarkTests>();
