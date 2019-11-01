# Introduction

This solution contains examples of how to upload and download files using a variety of .NET Core projects, including an Azure Functions example, a Web App example and lastly a Web Api example.

These examples can be run locally using the Azure Storage Emulator.

## FunctionApp1

This is an Azure Function example.

Your `local.settings.json` should look like the below:

```
{
    "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "AzureStorage:ConnectionString": "UseDevelopmentStorage=true",
    "AzureStorage:FilePath": "files"
  }
}
```

## WebApplication1

This is a .NET Core Web App example.

Your `appsettings.Development.json` should look like the below:

```
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "System": "Information",
      "Microsoft": "Information"
    }
  },
  "AzureStorage:ConnectionString": "UseDevelopmentStorage=true",
  "AzureStorage:FilePath": files"
}
```

## WebApplication2

This is a .NET Core Web Api example.

Your `appsettings.Development.json` should look like the below:

```
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "System": "Information",
      "Microsoft": "Information"
    }
  },
  "AzureStorage:ConnectionString": "UseDevelopmentStorage=true",
  "AzureStorage:FilePath": files"
}
```