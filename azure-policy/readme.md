Overview
========
In this toturial, we will be deploy a poilcy to check if SQL Servers has firewall rules deployed and only allows traffic from a specific VNet.

# Create Template
## Get Contributor Role Id
-   Install Azure CLI if not already

-   Login to Azure environment

```shell
az login
```

-   Select Azure Subscription

```shell
az account set -s "<SUBSCRIPTION_NAME>"
```

-   List Contributor Role

```shell
az role definition list --name Contributor
```

-   Output should look similar to below. Note down \<Role Id\>, we need this Id later

```json
[
  {
    "assignableScopes": [
      "/"
    ],
    "description": "Lets you manage everything except access to resources.",
    "id": "/subscriptions/<Subscription_id>/providers/Microsoft.Authorization/roleDefinitions/<Role Id>",
    "name": "<NAME>",
    "permissions": [
      {
        "actions": [
          "*"
        ],
        "dataActions": [],
        "notActions": [
          "Microsoft.Authorization/*/Delete",
          "Microsoft.Authorization/*/Write",
          "Microsoft.Authorization/elevateAccess/Action",
          "Microsoft.Blueprint/blueprintAssignments/write",
          "Microsoft.Blueprint/blueprintAssignments/delete"
        ],
        "notDataActions": []
      }
    ],
    "roleName": "Contributor",
    "roleType": "BuiltInRole",
    "type": "Microsoft.Authorization/roleDefinitions"
  }
]
```

## Update Template to fit actual needs

-   Update \<START_IP\> and \<END_IP\> to corresponding IP addresses

```json
    "properties": {
        "startIpAddress": "<START_IP>",
        "endIpAddress": "<END_IP>"
    }
```

-   Update \<ROLE_ID\> to the Role Id you copied from previous steps. Update Subscription ID to target subscription's ID

```json
    "roleDefinitionIds": [
    "/subscriptions/<SUBSCRIPTION_ID>/providers/Microsoft.Authorization/roleDefinitions/<ROLD_ID>",
    "/providers/microsoft.authorization/roleDefinitions/<ROLD_ID>"
    ],
```

-   Update Parameters to corresponding values if you want to deploy via portal, or either create a paramater file and use CLI to deploy.

```json
    "parameters": {
            "serverName": {
            "type": "String"
        },
        "vnetId": {
            "defaultValue": "/subscriptions/<SUBSCRIPTION_DI>/resourceGroups/<RESOURCE_GROUP_NAME>/providers/Microsoft.Network/virtualNetworks/<VNET_NAME>",
            "type": "String"
        }
    },
```

# Deploy to Azure

# Reference

- [Azure Policy SQL Firewall Rules sample](https://github.com/Azure/azure-policy/blob/master/samples/SQL/audit-sql-server-firewall-rule/azurepolicy.json)

- [Sample Policy](https://github.com/Azure/azure-policy/blob/master/samples/SQL/deploy-sql-server-auditing/azurepolicy.json)

- [How to remediate resources](https://docs.microsoft.com/zh-tw/azure/governance/policy/how-to/remediate-resources)

- [DeployIfNotExists effect](https://docs.microsoft.com/zh-tw/azure/governance/policy/concepts/effects#deployifnotexists-example)
