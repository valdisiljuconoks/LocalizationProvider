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

## Results

```
AMD Ryzen 9 3900X / .NET 10.0.8 / Release

                                         | baseline | after #1..#5 | delta
string-key, exact culture (cache hit)    | 1.431 us |    188 ns    | -87%
string-key, fr-BE -> fr fallback walk    | 1.417 us |    257 ns    | -82%
expression-key, exact culture (cache hit)| 3.242 us |    203 ns    | -94%

                                         | baseline | after #1..#5 | delta
string-key, exact culture                | ~1.5 KB  |    344 B     | -77%
string-key, fr-BE -> fr fallback walk    | 1888 B   |    376 B     | -80%
expression-key, exact culture            | 2408 B   |    344 B     | -86%
```

Five commits land each fix individually so the cumulative effect is
visible in `git log -p common/perf/`:

- `#1`  drop ToLower() from cache layer
- `#4`  cache assembled query/command handler chains
- `#3`  eliminate LINQ allocations in fallback chain walk
- `#2`  memoize expression-based resource keys
- `#5`  targeted cache invalidation (Insert no longer evicts the all-resources dictionary)
