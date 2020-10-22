# DotInitializr

DotInitializr is a simple web UI to generate an initial project structure from any project template in your git repo.  

It allows you to add input fields (textbox, dropdowns, radios, checkboxes) to customize your project metadata, such as the project namespace, package versions, inclusion of code snippets or files, by using [Mustache](https://mustache.github.io/mustache.5.html) notation paired with a JSON configuration file in the project template.

## How to Register a Template

Add the template metadata in `appsettings.json`:

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

