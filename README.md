# DotInitializr

[![NuGet](https://img.shields.io/nuget/v/DotInitializr.svg?style=flat-square)](https://www.nuget.org/packages/DotInitializr/)

DotInitializr is a simple web UI to generate an initial project structure from any project template in your git repo.

It lets you add input fields (textbox, dropdowns, radios, checkboxes) to customize your project metadata, using either [dotnet template](https://github.com/dotnet/templating/wiki/Reference-for-template.json) or [Mustache](https://mustache.github.io/mustache.5.html) notation, paired with a JSON configuration file in the project template.  For this configuration file, you hav a choice of using the dotnet's `template.json`, or a much simpler `dotInitializr.json`.

## Get Started

### Run the API only

Add the Nuget package **DotInitializr** to your ASP.NET Core 3.x project, and include the following in the `ConfigureServices`:

```c#
services.AddDotInitializr(Configuration);
```

#### Metadata Reader

Use the HTTP POST `api/metadata` endpoint to read the template metadata from a git repo. Specify the project repo information in the JSON body, for example:

```json
{
  "Name": "Steeltoe Template (dotnet)",
  "Description": "Demo project for Steeltoe",
  "SourceType": "git",
  "SourceUrl": "https://github.com/dsuryd/DotInitializr",
  "SourceDirectory": "DotInitializr.UnitTests\\TestTemplate_Steeltoe_DotNet",
  "SourceBranch": "master"
}
```

To access the service directly, inject `ITemplateMetadataReader`:

```csharp
public interface ITemplateMetadataReader
{
  TemplateMetadata GetMetadata(AppConfiguration.Template template);
}
```

#### Project Generator

Use the HTTP POST `api/generator` endpoint to generate a project. Specify the project repo and the metadata values in the JSON body, for example:

```json
{
  "projectName": "Starter",
  "templateType": "mustache",
  "templateSourceType": "git",
  "templateSourceUrl": "https://github.com/dsuryd/DotInitializr",
  "templateSourceDirectory": "DotInitializr.UnitTests\\TestTemplate",
  "templateSourceBranch": "master",
  "tags": {
    "projectName": "Starter",
    "namespace": "Starter",
    "ui": "React",
    "grpc": false,
    "react": true
  },
  "filesToExclude": "Services/**,Proto/**,ClientApp{{ng}}/**"
}
```

To access the service directly, inject `IProjectGenerator`:

```csharp
public interface IProjectGenerator
{
  ProjectMetadata BuildProjectMetadata(Dictionary<string, string> formData, TemplateMetadata metadata, AppConfiguration.Template template);
  byte[] Generate(ProjectMetadata metadata);
}
```

### Run the Website

Fork or download this repo and run it with Visual Studio 2019. The UI uses Blazor WebAssembly 3.2, so you'll need at least .NET Core SDK 3.1.300.

#### How to Register a Template

Add the template info to `appsettings.json`:

```json
{
  "DotInitializr": {
    "Templates": [
      {
        "Name": "ASP.NET Core SPA Template",
        "Description": "ASP.NET Core SPA template with Angular or React",
        "SourceType": "git",
        "SourceUrl": "https://github.com/dsuryd/DotInitializr",
        "SourceDirectory": "DotInitializr.UnitTests\\TestTemplate",
        "SourceBranch": "master"
      }
    ]
  }
}
```

## How to Configure Your Template Metadata

> If you choose to use dotnet's `template.json`, refer to the [dotnet template schema](https://github.com/dotnet/templating/wiki/Reference-for-template.json).

Add `dotInitializr.json` to the project root. The configuration is divided into 3 array groups:

```
{
  "TemplateType": "dotnet|mustache"
  "Tags": [],
  "ConditionalTags": [],
  "ComputedTags": []
}
```

### Tags

Use a tag to display a textbox, dropdown, or radio group in the Project Metadata section of the UI. The input value will be used to replace `tag_key` (dotnet) / `{{tag_key}}` (mustache) in your project template. Example:

```json
{
  "Tags": [
    {
      "Key": "NameSpace",
      "Name": "Project Namespace",
      "DefaultValue": "MyCompany.MyNamespace",
      "ValidationRegex": "^[\\w\\-. ]+$",
      "ValidationError": "Must be valid namespace"
    },
    {
      "Key": "PackageVersion",
      "Name": "Package Version",
      "DefaultValue": "3.0.0",
      "RadioOptions": ["2.4.4", "3.0.0"]
    },
    {
      "Key": "TargetFrameworkVersion",
      "Name": "Target Framework Version",
      "DefaultValue": "netcoreapp3.1",
      "Options": ["netcoreapp2.2", "netcoreapp3.1"]
    }
  ]
}
```

The `projectName` tag is added by default.

#### Casing

To change casing of a tag value, append `__lower` or `__upper` to the tag key in your source code.

#### Regex Matching

If matching by tag key isn't possible, you can specify a _Regex_ property. Example:

```json
{
  "Tags": [
    {
      "Key": "AspNetCoreVersion",
      "Name": "ASP.NET Core Version",
      "DefaultValue": "3.1.0",
      "Regex": "<PackageReference Include=\"Microsoft.AspNetCore.App\" Version=\"([0-9|.]+)+\" />"
    }
  ]
}
```

### Conditional Tags

Use a conditional tag to display a checkbox in the Dependencies section of the UI. The checkbox state will be used to include or exclude a code section in your projecte template inside the following tags:

- `#if tag_key` and `#endif` (dotnet; for C# code)
- `<!--#if tag_key-->` and `<!--#endif-->` (dotnet; for HTML/XML)
- `"#if tag_key": ""` and `"#endif tag_key": ""` (dotnet; for JSON)
- `{{#tag_key}}` and `{{/tag_key}}` (mustache)

Example:

```json
{
  "ConditionalTags": [
    {
      "Name": "RabbitMQ",
      "DefaultValue": false,
      "Description": "Add RabbitMQ connectors"
    },
    {
      "Name": "Redis",
      "DefaultValue": false,
      "Description": "Add Redis connectors",
      "FilesToInclude": "RedisExample1.cs,RedisExample2.cs"
    }
  ]
}
```

Use the `FilesToInclude` property to specify comma-delimited paths to include when the tag value is true. To include all files in a folder, use `folder_name/**`.
If a conditional tag key is used on a file name, it will be removed from the name when the file is included.

### Computed Tags

Use a computed tag to derive a boolean value from the other tags with a logical expression. Example:

```json
  "Tags": [
    {
      "Key": "ui",
      "Name": "UI",
      "DefaultValue": "React",
      "RadioOptions": ["Angular", "React"]
    }
  ],
  "ConditionalTags": [
    {
      "Name": "MongoDB",
      "DefaultValue": false,
      "Description": "Add MongoDB connectors"
    },
    {
      "Name": "MySql",
      "DefaultValue": false,
      "Description": "Add MySql connectors"
    },
  ],
  "ComputedTags": [
    {
      "Key": "ng",
      "Expression": "ui == \"Angular\"",
      "FilesToInclude": "ClientApp{{ng}}/**"
    },
    {
      "Key": "react",
      "Expression": "ui == \"React\"",
      "FilesToInclude": "ClientApp{{react}}/**"
    },
    {
      "Key": "AnyDB",
      "Expression": "MongoDB || MySql",
      "FilesToInclude": "Models/**"
    },
  ]
}
```

#### Expression Functions

The expression supports various functions such as `count()`. For example:

```json
"Expression": "count(MongoDB, MySql) > 1"
```

| Function              | Description                                           |
| --------------------- | ----------------------------------------------------- |
| `count(param bool[])` | Count the number of conditional tags with true value. |
| `xmlEncode(string)`   | XML encode a tag value.                               |
| `upperCase(string)`   | Converts tag value to upper case.                     |
| `lowerCase(string)`   | Converts tag value to lower case.                     |
| `titleCase(string)`   | Converts tag value to title case.                     |
| `kebabCase(string)`   | Converts tag value to kebab case.                     |
