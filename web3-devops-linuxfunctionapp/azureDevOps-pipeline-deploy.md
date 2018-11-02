Overview
========

In this document we will be demonstrating deploy a NodeJS Function App with Web3
(and other NodeJS packages) to Azure Function on Linux by Azure DevOps Build and
Release pipeline. This is particularly useful while creating a blockchain
application on Azure as one of a common challenge we were experienced is to
install node packages onto those PaaS environments.

Before creating pipelines, we assume you have

-   Created an empty Javascript Azure Function on Linux

-   Have an working Blockchain environment for testing

-   Have Function App source code push to Azure Repos with below structure

![](media/3d251e163c3f88626d0b6f3143ee5e44.png)

Create Build Pipeline
=====================

-   Go to Azure DevOps portal, create a new Build pipeline. We will use Build
    pipeline to setup NodeJS packages

![](media/3b8d82584f32d06f7f8788b4d29ff7ac.png)

-   In this demo, my source codes are pused to Azure Repos, you can use GitHub
    as well. So select Azure Repos Git has my source codes here.

![](media/f7ffbadfa2a01058173d9a6d6bab5c3b.png)

-   For simplicity sake, I am using C\# function template as a base pipeline
    template

![](media/86713e6caf9915191123be9108c7891d.png)

-   Now since we are to deploy to Linux on Azure Function, here we will be using
    Ubuntu agent

-   We also want to remove some unused tasks, such NuGet restore, NuGet 4.4.1
    task.

![](media/c2dd33ce8fa36411cba791bfbbf8e9b1.png)

-   Add a npm task to the pipeline

![](media/2bc9cbfa4ad5e1f32765087a95758e28.png)

-   Set command to “install”, which is the default

![](media/efec95796066ad2210316fe87ec5d396.png)

-   Now change package.json folder to sample

![](media/17ece8e038ebc3625ef503d8f0ea65da.png)

-   Change Root Folder of archive file to functionApps folder and DO NOTcheck
    “Prepend root folder name to archive paths”

![](media/f17647c28401c9b619ec2d611bf0298c.png)

-   Add another task to specify NodeJS version

![](media/f0050bdc049489e46732e9de24f1ee81.png)

-   We will be using NodeJS version 8.11.X here.

![](media/1402ff5b156b56cb3e955d20c6c91e3a.png)

-   You pipeline should be similar to this

![](media/1f9011898ac5ba7ac4d5c18a789fd617.png)

-   Save and Queue.

![](media/664f22cc135cfb3942f3755279705699.png)

Create Release Pipeline
=======================

-   Create a new Release pipeline with Azure App Service deployment template

![](media/5d6527fdd1868f3ad90aae3a18a05275.png)

-   Select correct Subscription and App Type, you may need to manually input App
    Service name if it’s not present

![](media/d7bea59f5bf7ce134f40c7a211ddd2a0.png)

-   Go to Deploy Azure App Service Task, change version to 4.\* (Preview), leave
    others default

![](media/10441d86313082b46f17b821464409b1.png)

-   Save the Stage and add Artifact

![](media/f5789de7f3933fb4d203398456793e29.png)

-   Choose artifact from the last build

![](media/4fc6be565cde285663798af3fd557a91.png)

-   Save and create a release to kick-off deployment
