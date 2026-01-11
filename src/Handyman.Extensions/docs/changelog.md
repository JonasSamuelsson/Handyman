# Handyman.Extensions changelog

## 3.0.0 - 2026-01-11


* Add net8.0, net9.0 TFMs
* Remove net472, net48, net6.0, net7.0 TFMs
* Removed extension methods that duplicate framework functionality:
  * `Chunk<T>(int)`, `Clamp<T>`, `Contains(string, StringComparison)`, `GetValueOrDefault<TKey, TValue>`, `Shuffle<T>`,`SkipLast<T>`, `TakeLast<T>`, `ToSet<T>`, `TryAdd<TKey, TValue>`, `TryGetFirst<T>`

## 2.7.2 - 2024-07-05

* Performance improvements

## 2.7.0 - 2024-04-08

* Add `IEnumerable<T>.AsReadOnlyCollection()`.
* Add `IEnumerable<T>.AsReadOnlyList()`.
