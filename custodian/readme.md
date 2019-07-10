Table of Contents
=================

[Installation](#Installation)

-   [Install Azure Cli](#Install-Az-Cli)

-   [Install Custodian and Azure plugin](#Install-Custodian)

[Validate installation](#Validation)

[Sample Policies](#Sample-Policies)


# Installation

## Install Az Cli

-   Install Azure Cli by runing below command

```shell
curl -sL https://aka.ms/InstallAzureCLIDeb | sudo bash
```

-   Login to Azure

```shell
az login
```

## Install Custodian

-   In your Ubuntu 18.04 TLS Machine, clone source codes form github

```shell
git clone https://github.com/cloud-custodian/cloud-custodian.git
```

-   Install pip if not already

```shell
sudo apt-get install python-pyp
```

-   Install VirtualEnv

```shell
pip install virtualenv
```

-   Create a Python3 VEnv

```shell
virtualenv -p python3 .py3env
```

-   Activate it

```shell
source .py3env/bin/activate
```

-   Build and Install Custodian and C7N_Azure plugin

```shell
cd cloud-custodian
pip install -e cloud-custodian/.
pip install -e cloud-custodian/tools/c7n_azure/.
```

# Validation
## Periodically tag Stopped VMs

To validate the installation, we want to deploy a simple policy to Azure Function and have Custodian periodically tag stopped VMs in our subscription

-   In order to have this work, first we need to set environment variables

```shell
# select correct subscription
az account set -s "[YOUR SUBSCRIPTION NAME]"

# create service principal
az ad sp create-for-rbac --name [SP NAME] --password [SP PASSWORD]
```

-   You'll see below output

```json
{
  "appId": appid,
  "displayName": name,
  "name": name,
  "password": password,
  "tenant": guid
}
```

-   In your Ubuntu machine, set below environment vaeriables. Custodian automatically capture these variables during deployment.

```shell
export AZURE_TENANT_ID=tenant
export AZURE_SUBSCRIPTION_ID=subscriptionId
export AZURE_CLIENT_ID=appId
export AZURE_CLIENT_SECRET=password
```

-   Create our policy file

```yaml
policies:
  - name: stopped-vm
    mode:
        type: azure-periodic
        schedule: '0 * * * * *'
        provision-options:
          servicePlan:
            name: functionshost
            location: East Asia
            skuTier: Standard
            skuName: S1
          appInsights:
            location: East Asia
          storageAccount:
            name: sampleaccount
            location: East Asia
    resource: azure.vm
    filters:
      - type: instance-view
        key: statuses[].code
        op: not-in
        value_type: swap
        value: "PowerState/running"
    actions:
      - type: tag
        tag: "tag-by-function"
        value: "true"
```

-   Run below command to deploy

```shell
custodian run --output-dir ./output policy.yaml
```

-   This will take around 5 minutes to have Function successfully deployed. Once deployed, wait until it gets triggeted or manually execute it via portal. Stopped Virutal Machines will be tagged.

# Sample Policies

##  Azure Function Samples

-   [Tag Stopped VMs](policies/tag-stopped-vm.yaml)

##  Event Grid Samples

-   [Tag VMs by CPU utlization](policies/event-grid.yaml)

##  Mailer Samples

- When CPU utlization is low, send an email notification to specific email address 
  - [Mailer configureation file](policies/mailer/mailer.yaml)
  - [Policy Yaml file](policies/mailer/vmcpu-notificatgion.yaml)
  - To run this sample, You must first create a Service Principal and run below commands to set environment varibles
```
export AZURE_TENANT_ID=xxx
export AZURE_SUBSCRIPTION_ID=xxx
export AZURE_CLIENT_ID=xxx
export AZURE_CLIENT_SECRET=xxx
```
  - You need to assign Storage account's "Storage Queue Data Contributor" role to the service principal
  - Configure mailer then deploy policy to Azure Function
```
custodian run --output-dir ./output ./vmcpu-notificatgion.yaml
c7n-mailer --config mailer.yml 
```