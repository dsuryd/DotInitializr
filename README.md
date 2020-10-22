# DotInitializr

DotInitializr is a simple web UI to generate an initial project structure from any project template in your git repo.  

It allows you to add input fields (textbox, dropdowns, radios, checkboxes) to customize your project metadata, such as the project namespace, package versions, inclusion of code snippets or files, by using [Mustache](https://mustache.github.io/mustache.5.html) notation paired with a JSON configuration file in the project template.

## How to Register a Template

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
        "SourceDirectory": "DotInitializr.UnitTests\\TestTemplate"
      }
    ]
  }
}
```

## How to Configure Metadata Inputs

Add `dotInitializr.json` to the project root. The configuration is divided into 3 array groups:
```
{
  "Tags": [],
  "ConditionalTags": [],
  "ComputedTags": []
}
```

### Tags

Use a tag to display a textbox, dropdown, or radio group in the Project Metadata section of the UI.  The input value will be used to replace the Mustache tag `{{tag_key}}` in your project template.  Example:
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

### Conditional Tags

Use a conditional tag to display a checkbox in the Dependencies section of the UI.  The checkbox state will be used to include or exclude a code section between the Mustache tag `{{#tag_key}}` and `{{/tag_key}}` in your project template.  Example:
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
    },
  ]
}
```

Use the `FilesToInclude` property to specify comma-delimited files to include when the tag value is true. To include all files in a folder, use `folder_name/**`.
If a conditional tag key is used on a file name, it will be removed from the name when the file is included.

### Computed Tags

Use a computed tag to derive a boolean value from the other tags with a logical expression.  Example:
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
    ,
    {
      "Key": "AnyDB",
      "Expression": "MongoDB || MySql",
      "FilesToInclude": "Models/**"
    },
  ]
}
```

The expression supports `Count()` custom function to count the number of conditional tags with true value.  Example:
```json
"Expression": "Count(MongoDB, MySql) > 1"
```
