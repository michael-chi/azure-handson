Overview
========

Binding is a powerful feature introduced in Azure Function Apps where as
a developer we only need to specify where the data source is, and what's
the query statement. Function App runtime will do that query for us
instead of us to understand connection and data access details.

To leverage power of Binding, we need to install Binding extensions to
our Function App host. Official instruction on how to do this manually
is documented here:
<https://github.com/Azure/azure-functions-host/wiki/Updating-your-function-app-extensions>

In this document I will be demonstrating how to create a Azure DevOps
Build and Release pipeline to deploy a Node.Js Azure Function Apps with
CosmosDB Binding.

Create Azure Function
=====================

-   Create an empty C\# Azure Function App in Azure

![](.//media/image1.png)
-   Go to "Application Settings", add a CosmosDB connection setting.

    In my sample, the connection string is named
    "serverlessjo13\_DOCUMENTDB", you may have a different one.

![](.//media/image2.png)

Create Cosmos DB
================

-   Create a CosmosDB account and a database as well as a collection
    under it

-   Add a sample document to the collection for our further tests

![](.//media/image3.png)

Create Azure DevOps project
===========================

-   In Azure DevOps Portal, create a repro for our codes and commit our
    Function App codes to it. In order to have our pipeline works, here
    I have a folder structure as below.

    -   HttpTrigger1 is our Function, in side it are our Node.Js codes

    -   Extensions.csproj contains binding extensions we want to install

![](.//media/image4.png)
-   A sample extensions.csproj looks like this.
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>netstandard2.0</TargetFramework>
    <WarningsAsErrors></WarningsAsErrors>
    <DefaultItemExcludes>**</DefaultItemExcludes>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.Azure.WebJobs.Extensions.DurableTask" Version="1.6.2" />
    <PackageReference Include="Microsoft.Azure.WebJobs.Extensions.CosmosDB" Version="3.0.1"/>
    <PackageReference Include="Microsoft.Azure.WebJobs.Extensions.Storage" Version="3.0.0" />
    <PackageReference Include="Microsoft.Azure.WebJobs.Script.ExtensionsMetadataGenerator" Version="1.0.1" />
  </ItemGroup>
</Project>

```
Create Build Pipeline
=====================

-   Go to Pipeline, Build, create a new Build pipeline

![](.//media/image5.png)

-   Select your repository and branch then continue

![](.//media/image6.png)

-   Use C\# Function template

![](.//media/image7.png)

-   Once selected, than we want to add a "Dotnet Core" task to the
    pipeline. We will be using this task to "build" our extensions
    project which than restores all dependencies required including
    binding extensions

![](.//media/image8.

-   Now we want to disable or remove some tasks that are not used in
    this pipeline.

![](.//media/image9.png)

-   Your pipeline should looks like this

![](.//media/image10.png)

-   Next, we are to configure several tasks

    -   We want to "restore" required dependencies including Binding
        extension

    -   We want to archive (zip) all required files, including our
        source codes and dependencies for our release pipeline

-   Go to the dotnet core task

    -   Set "Command" to build (which is the default)

    -   Set "Path to project(s)" to \*\*/extensions.csproj

    -   Add below "Arguments"
```
        -o bin --no-incremental --packages .nuget
```
    -   Check "Working Directory" is set to \$(Agent.TempDirectory)

![](.//media/image11.png)

-   Go to Archive Task

    -   Set "Root folder or file to archive" to wwwroot

        -   This is our root folder in Azure DevOps Repro

![](.//media/image12.png)

-   Save and Queue

Create Release Pipeline
=======================

-   Create a new Release pipeline with Azure App Service Deployment
    template

![](.//media/image13.png)
-   Add artifact

![](.//media/image14.png)

-   Choose the latest build we created above

![](.//media/image15.png)

-   Once created, go to Deployment Process than make sure we selected
    correct subscription, App type and Function App

![](.//media/image16.png)

-   Leave everything in "Azure App Service Deploy" default

![](.//media/image17.png)

-   Save

Verify
======

-   Now let's create a new release to verify

![](.//media/image18.png)

-   Check you selected correct Build to Release

![](.//media/image19.png)

-   Wait until it completed, you should see Function App been deployed
    to Azure

![](.//media/image20.png)

-   If you'd like to automate everything, go to Artifacts and enable CD
    trigger. This will kick-off release pipeline when there is a new
    Build available

![](.//media/image21.png)

-   Go to the newly deployed Function App on Azure Portal. Check if
    correct cosmosDB connection string are selected

![](.//media/image22.png)

-   If you go to Kudu website, folder structure should look like below.
    Where HttpTrigger1 is our function, bin folder contains all required
    dependencies including Binding extensions.

![](.//media/image23.png)

-   Open up browser and verify

![](.//media/image24.png)
