Installation
============

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


