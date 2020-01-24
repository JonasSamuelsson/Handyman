# handyman-outdated

`handyman-outdated` is a [dotnet global tool](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools) for listing outdated dependencies.

Install using `dotnet tool install -g handyman-outdated`.

## Usage

```
Usage: handyman-outdated analyze [options] <path>

Arguments:
  path                         Path to folder or project

Options:
  --output-file <OUTPUT_FILE>  Output file(s), supported format is .md
  --tags <TAGS>                Tags filter, start with ! to exclude
  --no-restore                 Skip dotnet restore
  -v|--verbosity <VERBOSITY>   Verbosity
                               Allowed values are: Normal, Quiet, Minimal
  -?|-h|--help                 Show help information
```

How projects are analyzed can be controled by placing a `.handyman-outdated.json` file next to the project file or in any of its parent directories.

``` json
{
    "SchemaVersion": "1.0",
    "IncludeTransitive": false,
    "Skip": false,
    "Tags": [ "foo" ],
    "TargetFrameworks": [
        {
            "Name": "netstandard*",
            "Packages": [
                {
                    "Name": "System.Xml*",
                    "IgnoreVersion": "4.5.*"
                }
            ]
        }
    ]
}
```
