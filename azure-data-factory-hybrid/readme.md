Table of Content
================

[Overview](#overview)

[Setup Virtual Network](#setup-virtual-network)

[Setup Virtual Machine](#setup-virtual-machines)

[Create VNet Peering](#create-vnet-peering)

[Setup File Server](#setup-file-server)

[Create Data Factory Pipeline](#create-data-factory-pipeline)

[Integration Runtime High Availability](#integration-runtime-high-availability)

Overview
========

Below is a high-level architecture of this experiment.

-   Two Virtual Network, Local-Net represents on-prem network environment,
    Cloud-Net represents cloud network environment. Each connected via VNet
    Peering to simulate VPN connection.

-   Two Machines in Cloud-Net, both with Integration Runtime installed to enable
    high availability

-   In Local-Net, a Windows Server 2019 Machine is created which serves as File
    Server.

-   Azure Data Factory get file content in that file server via Integration
    Runtime then insert data into a Storage table

![](media/e64a6f375d76f8da377f397f6d4afcde.png)

Setup Virtual Network
=====================

-   We will be creating two virtual networks. Each will be connecting to each
    other via VNet Peering.

-   Clout-VNet

![](media/f493d2857435b0ff1ed5590051ec0d45.png)

-   Local-VNet

![](media/6db5e2467ae2b1de73fd9c774095aa33.png)

Setup Virtual Machines
======================

-   We will need two Windows Server 2019 Machines, one in Cloud-Vnet, one in
    Local-VNet.

>   Win-FS: Windows Server 2019 File Server

>   Cloud-Agent: Windows Server 2019 for Integration Runtime

Create VNet Peering
===================

To simplify environment configuration, we use VNet Peering here to establish
connection between a VNet in Southeast Asia and another VNet in East Asia

The creation process is quite straight forward, follow [these
steps](https://docs.microsoft.com/zh-tw/azure/virtual-network/virtual-network-manage-peering)
to setup Peering.

Setup File Server
=================

-   From Server Manager, install required File Server services

![](media/3b972452f15acbbaabda584392812c10.png)

-   If this is the first time you install File server, refresh from Server
    Manager then restart Server Manager

![](media/03c8633e260ce2ddecb4b1feb094ebca.png)

-   Go to File and Storage Service, Right click on the Disk you’d like to share,
    Create a new share

![](media/3ce977f1137694977b62cf15685928b8.png)

-   Quick Setup

![](media/84c67ff6b917883c2faf96823391da52.png)

-   Name the share

![](media/7a96ea8904c441227fc7211cccbba7a3.png)

-   Specify authentication mode

![](media/c5eabb5a6478b0596c626fad9d8f3d49.png)

-   Share permissions

![](media/e94dddeca995c6366def7cb0ff2a898f.png)

Create Data Factory Pipeline
============================

-   Copy Data template

-   Name the task

![](media/d5f9a8a1669e8e3f7aafc0a6eb253ca5.png)

-   Add a new connection

![](media/2941cbc61af00a58e3726951df727de0.png)

-   Choose File System

![](media/c761707fe2d123d4fbe83873fa42a467.png)

-   Fill in required information, such as File share path, user identity…etc

![](media/417de994041c643248bffcae14381cba.png)

-   Now add a new Integration Runtime

![](media/22c9fff172d484bcc1590b7025699586.png)

-   Choose “Self-Hosted” runtime since we will need the runtime running within
    the VNet

![](media/9a29332eb2e86a433c664548e028dcc3.png)

-   Click Next until you see this screen. Note down these keys for later uses

![](media/a17d322a2022ef7bb885be51b5f5a847.png)

-   Download the integration runtime to your file server or another server in
    Cloud-Agent machine that has access to the file share. When prompt to input
    key, fill in one of the keys noted above.

-   Finally, enable remote access so that later we can configure
    high-availability for the agent

![](media/569f4c90c4215bb35c5f8f02f4285a4e.png)

-   Specify intranet port here

![](media/36ba8894ad1de164f59b4bf29eb469ca.png)

-   Do not forget to open port in your firewall

![](media/193da4858be63000b1fc2e282273b6fb.png)

-   Go back to Azure Data Factory, you can now test connection, it should
    succeed

![](media/3fd3977e9fb9f1593eede452ce19bc55.png)

-   Once verified connectivity, click Finish. This will bring us back to Data
    Source page. Choose the File Share connection we just created and click
    Next.

-   Specify desired start time and end time, file names to pick up.

    -   DO NOT check Binary Copy since we want to parse incoming files.

    -   If you want to handle all files in that share, leave file names field
        empty

![](media/b631b26a218041d92a0ed3017b8d3ab6.png)

-   Specify correct file format and click “Detect format”, you should see file
    gets parsed and a preview should be shown. If you see an error like below,
    check and uncheck “Column names in the first row”

![](media/e16bcb4af1c0aa2caa8ea239f5dbc8f8.png)

-   You can switch to Schema tab to change column data type

![](media/628a8c033d5f336d005049c1a0d083ab.png)

-   Click Next when ready

-   Now choose Storage Table as Destination

![](media/27749f3849d85a9895c2cefa7e59fbc2.png)

-   Specify table name

![](media/1ebeb1d70a1835330cdd181a389b171e.png)

-   Update Mapping

![](media/8921ceb4ed0d345c57de1369eaa232a9.png)

-   Next then Finish the wizard

-   Publish the pipeline by clicking “Publish All” button

![](media/79f238ca8da689bd28826a45f5b3042d.png)

-   Go to Monitor you should see the pipeline running

![](media/6d90ed9169b55b9a67069f8094d91330.png)

Integration Runtime High Availability
=====================================

In order to keep our pipeline up and running, our Integration Runtime has to be
highly available. Integration Runtime has HA in-place. To join a new agent node
to existing Integration Runtime Nodes, simply install the runtime to another
machine, specify the same Key in the wizard, and that’s it.

![](media/33eb17ab3a3aad41718c2f7f561ff1c3.png)
