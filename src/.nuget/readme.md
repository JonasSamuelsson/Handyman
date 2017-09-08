Use the following commands to pack & push nuget packages

```
dotnet pack -c release --include-symbols
dotnet nuget push -k {api-key} -s https://nuget.org .\bin\release\{project}.nupkg
```