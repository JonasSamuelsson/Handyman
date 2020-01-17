# handyman-ado

This is a .NET Core global tool for working with Azure DevOps.

```
Usage: handyman-ado [command] [options]

Options:
  -?|-h|--help  Show help information

Commands:
  list-repos
```

```
Usage: handyman-ado list-repos [options]

Options:
  --host <HOST>                                         Host (defaults to dev.azure.com)
  --organization <ORGANIZATION>                         Organization
  --project <PROJECT>                                   Project
  -pat|--personal-access-token <PERSONAL_ACCESS_TOKEN>  PersonalAccessToken
  -of|--output-format <OUTPUT_FORMAT>                   OutputFormat
                                                        Allowed values are: List, Table, Json
  -?|-h|--help                                          Show help information
```
