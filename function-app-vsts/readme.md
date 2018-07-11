Overview
========

Visual Studio 2017 now come with Azure Function Tools enables you to develop
Azure Function App locally. The tool supports library type of Function app
development which is not compatible with Function App’s source control based
CI/CD options.

In order to create our CI/CD pipeline for Azure Function App. We will be
leveraging VSTS to automatically deploy to Azure when new codes are committed.

Create Function App in VS 2017
==============================

We are not going to detail those steps to create local Function App in VSTS
here. You can refer to [official
document](https://docs.microsoft.com/en-us/azure/azure-functions/functions-develop-vs)
for instruction. The structure of our VS 2017 solution looks like below.

![](media/350fd1fde41ffa4651314f7ab2eca060.png)

You need to commit these codes to your VSTS repository

![](media/e371d38207f277e1e4ffe4be3ce187d7.png)

Create Build Definition
=======================

-   Goto Build and Release, then create a new definition

![](media/a406d4b7a5d2b781de97700743aac743.png)

-   You choose correct VSTS repository and branch, then click “Continue”

![](media/272b7fef7ff4beb2d6e76f4ce4ae39c4.png)

-   Let’s use Azure App Service template

![](media/2c29d64fc497e32961844f70b4cd98b0.png)

-   Once selected, you will be shown a default template looks like below. Click
    the Azure App Deploy task we just selected.

![](media/0bf5768b6b3c717b8c3a04028445cb04.png)

-   Choose Azure Subscription which the Function App will be deploying to

![](media/0e3d91201e123d4392bc802c16688d07.png)

-   Fill in required information.

    Note that value of “Package or folder” should be
    \$(build.artifactstagingdirectory)/\*\*/{PACKAGE_NAME}.zip.

    Since here we have below packages as shown in VSTS solution, we will be
    creating totally three Azure App Service Deploy tasks for each package

    -   CreateRatingV1

    -   GetRatingV1

    -   GetRatingsV1

![](media/10f94621f7a177f1258f9a3b4f27d1fc.png)

-   Add another deploy task for GetRatingsV1 package

![](media/121fa5e55a7706386afab96d9f194251.png)

-   Your task should look like this.

![](media/2fd4646f24bc789024dad4e5f749d91e.png)

-   Add another one for GetRatingV1, your Build should looks like below

![](media/b7c29d89d3d75005c84ff13a87317180.png)

-   Goto Trigger, click Add

![](media/a06fe1ea8b59373c403ada9a661314ad.png)

-   Enable CI and make sure to select a branch to integrate with

![](media/27ecd8364acb2de0f75053cbfed8ea33.png)

-   Once completed, Save and Queue to verify result

![](media/7d3f0a28a8ad521d7bf8683393731c3d.png)

-   Once build and deploy completed we’ve successfully configured VSTS auto
    deployment. Once you commit and push changes to VSTS, it will trigger build
    process to build and deploy to selected Azure Function App.

Working with Function App Slot
==============================

Slot is in preview which enables you to create production/staging environment in
the cloud. In this section we will be modifying our VSTS Build to deploy latest
bits to Staging slot.

-   Slot is in preview, to enable Slot feature, goto Function App Settings

![](media/52179d8ade65a92f241eb60f6d9d5c86.png)

-   Create a new Slot namely “Staging”

![](media/a36ff6aa03493621c3cece3832289b82.png)

-   Go back to VSTS Build definition, Check “Deploy to Slot” option and choose
    the slot we created.

![](media/6b8b2936cc131e6c18204759e9a62177.png)

-   Make sure you update all three deploy tasks then Save.
