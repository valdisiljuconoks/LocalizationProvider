# DbLocalizationProvider.Benchmarks

Micro-benchmarks for the hot path that downstream sites hit on every page render —
`ILocalizationProvider.GetString(...)`.

## Running

```powershell
dotnet run --project perf/DbLocalizationProvider.Benchmarks -c Release -- --filter "*"
```

The project uses BenchmarkDotNet's in-process toolchain so it does not spawn a child
process (avoids antivirus interference on Windows). For faster iteration during
development pass `--maxIterationCount 10 --minIterationCount 5 --warmupCount 5`.

## Benchmarks

- **string-key, exact culture (cache hit)** — `GetString("known.key", en)` with the
  translation already in the cache. Measures the dispatcher + cache-hit + return
  path, no fallback walk.
- **string-key, fr-BE -> fr fallback walk (cache hit)** — translation exists only
  for `fr`, requested in `fr-BE`. Measures the parent-culture fallback walk on top
  of the cache hit.
- **expression-key, exact culture (cache hit)** — `GetString(() => Resources.X)`
  with `X` already in the cache. Measures expression-tree walking and resource-key
  resolution on top of the cache hit.

## Baseline (before performance fixes)

```
AMD Ryzen 9 3900X / .NET 10.0.8 / Release

| Method                                              | Mean     | Allocated |
|---------------------------------------------------- |---------:|----------:|
| string-key, exact culture (cache hit)               | 1.431 us |   ~1.5 KB |
| string-key, fr-BE -> fr fallback walk (cache hit)   | 1.417 us |   1888  B |
| expression-key, exact culture (cache hit)           | 3.242 us |   2408  B |
```

(The exact-culture row's `Allocated` column appears as `-` due to a BenchmarkDotNet
rounding quirk when the value is near the noise floor; the `Gen0` rate of 0.0439
collections per 1k ops corresponds to roughly 1-1.5 KB allocated per call.)
