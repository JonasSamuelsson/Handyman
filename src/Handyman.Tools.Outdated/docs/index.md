# handyman-outdated

[changelog](./changelog.md)

`handyman-outdated` is a [dotnet global tool](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools) for listing outdated dependencies.

Install using `dotnet tool install -g handyman-outdated`.

## Usage

`handyman-outdated` supports three different commands

* `analyze`
* `generate-config`
* `report`

### `analyze`

Analyzes one or multiple projects for outdated packages and outputs the result to disk.

```
Usage: handyman-outdated analyze [options] <path>

Arguments:
  path                         Path to folder or project

Options:
  --output-file <OUTPUT_FILE>  Output file(s), supported formats are json/md
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

### `generate-config`

Generates empty config file template.

```
Usage: handyman-outdated generate-config [options] <Output file>

Arguments:
  Output file

Options:
  -?|-h|--help  Show help information
```

### `report`

Converts json result file to markdown.

```
Usage: handyman-outdated report [options] <path>

Arguments:
  path                         Path to json result file

Options:
  --output-file <OUTPUT_FILE>  Output file(s), supported formats are json/md
  --tags <TAGS>                Tags filter, start with ! to exclude
  -?|-h|--help                 Show help information
```