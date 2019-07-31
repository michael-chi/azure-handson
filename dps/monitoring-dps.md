## Table of Contents

## Overview

Device Provisioning Service does not currently provide capabilities to subscribe to device provisioninig events. But we can archive this by adding event subscription to individul IoT Hubs.

In this toturial, I will be creating event subscription which listening to device creation and device deletion events, send those event metadata to a storage queue for further processing.

## Steps

-   Assume Storage account and Storage queue has been created.

-   Get IoT Hub Resource Id

```bash
FOR /F "tokens=*" %a in ('az iot hub show -n michi-dps-sea-001 -g michi-dps-20190726-rg --query "id" -o tsv') do SET IOTHUB_ID=%a
echo %IOTHUB_ID%

#/subscriptions/e35c484f-2d35-479f-8adb-9fe20c79394e/resourceGroups/michi-dps-20190726-rg/providers/Microsoft.Devices/IotHubs/michi-dps-sea-001
```

-   Retrieve Resource Id of Storage account and Construct Storage Queue resource Id

```bash
FOR /F "tokens=*" %a in ('az storage account list -g michi-postdemo-rg --query "[].{id:id}" -o tsv') do SET QUEUE_ID=%a/queueServices/default/queues/dps-device-registration
echo %QUEUE_ID%

#/subscriptions/e35c484f-2d35-479f-8adb-9fe20c79394e/resourceGroups/michi-postdemo-rg/providers/Microsoft.Storage/storageAccounts/botstatusdb/queueServices/default/queues/dps-device-registration
```

-   Create event subscription

```bash
az eventgrid event-subscription create \
    --name device-registration-events \
    --source-resource-id <IOT Hub Resource ID> \
    --endpoint-type storagequeue \
    --endpoint <STORAGE_QUEUE_RESOURCE ID> \
    --included-event-types Microsoft.Devices.DeviceCreated Microsoft.Devices.DeviceDeleted

# az eventgrid event-subscription create \
#     --name device-registration-events \
#     --source-resource-id /subscriptions/e35c484f-2d35-479f-8adb-9fe20c79394e/resourceGroups/michi-dps-20190726-rg/providers/Microsoft.Devices/IotHubs/michi-dps-eastasia-001 \
#     --endpoint-type storagequeue \
#     --endpoint /subscriptions/e35c484f-2d35-479f-8adb-9fe20c79394e/resourceGroups/michi-postdemo-rg/providers/Microsoft.Storage/storageAccounts/botstatusdb/queueServices/default/queues/dps-device-registration --included-event-types Microsoft.Devices.DeviceCreated Microsoft.Devices.DeviceDeleted
```

## References

[Event Grid - Subscrbes to IoT Hub events](https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-event-grid)
