Table of Content
================

[Overview](#overview)

[Before Start](#before-start)

[Prerequistite](#prerequisite)

- [Solution Provider](#solution-provider)

[Solution Provider Steps](#solution-provider-steps)

- [Setup IoT Hub and DPS](#setup-iot-hub-and-dps)

- [Configure Enrollment Group](#configure-enrollment-group)

[Hardware Manufacturer Steps](#hardware-manufacturer-steps)

- [Codes](#codes)

[References](#references)

## Overview

In this tutorial, I am simulating an IoT solution provider, which sell devices to different customers in different geo-locations, different industries. The company wants to automatically provision devices sold to their customers and control how and where the device should be provisioned from cloud.

Azure Device Provisioning Service is a powerful to support this scenario. Image below architecture.

    [root certificate] -> [intermediate certificate for customer A] -> [company A device certificates...]

                       -> [intermediate certificate for customer B] -> [company B device certificates...]

Devices with device certificates signed by intermediate certificate will automatically be assigned to an IoT Hub which has its root certificates associated with.

## Before Start
Keep in mind that this tutorial uses self-signed certificate, which should only be used in development or testing environment. For production environment, a trusted authority signed certificate should be used.

## Prerequisite

#### Solution Provider
- Resource Group: michi-dps-201907260-rg
- DPS: michi-dps-20190726
- IoT Hub: michi-dps-eastasia-001

DPS and IoT Hub already linked

- Clone [this repo](https://github.com/Azure/azure-iot-sdk-c.git)

#### Spec
- OS: Ubuntu 18.04
- OpenSSL: 1.1.1 11 Sep 2018
- azure-cli: 2.0.69
  - azure-cli-iot-ext: 0.7.1

## Solution Provider Steps

#### Setup IoT Hub and DPS

As Solution provider, we want to setup DPS and associate IoT Hub with it, so that later after we configured DPS, each device, when register themselves to DPS, will than dispatched to corresponding IoT Hub by DPS policy.

- Execute below steps to create DPS, IoT Hub and link them together.

```bash
```shell
# Install IoT Extension
az extension add --name azure-cli-iot-ext

# Create Resource Group
az group create --name michi-dps-20190726-rg --location eastasia

# Create IoT Hub
az iot hub create --name michi-dps-eastasia-001 --resource-group michi-dps-20190726-rg --location eastasia

# Create DPS
az iot dps create --name michi-dps-20190726 --resource-group michi-dps-20190726-rg --location eastasia

# Get Connection String
FOR /F "tokens=*" %a in ('az iot hub show-connection-string --name michi-iothub-dps-sea-001 --key primary --query connectionString -o tsv') do SET connString=%a
echo %connString%

# Associate IoT Hub with DPS
az iot dps linked-hub create --dps-name michi-dps-20190726 --resource-group michi-dps-20190726-rg --connection-string %connString% --location eastasia

# Verify
az iot dps show --name michi-dps-20190726
```

#### Configure Enrollment Group

- Device Provisioning Service use Enrollment Group to manage devices which has same configuration. These devices, when boot up, register themselves to DPS with their certificate singed by a root CA. DPS determine if that certificate is valid and assign connected device to corresponding IoT Hub.

- Execute below commands to setup Enrollment Group

```bash
# Clone IoT C SDK
git clone https://github.com/Azure/azure-iot-sdk-c.git

cd tools/CACertificates
chmod 700 certGen.sh
chmod 700 certGenEx.sh

# Create Root and Intermediate CA
./certGen.sh create_root_and_intermediate

## Chain
# Concat root and intermediate certificate
cat certs/azure-iot-test-only.intermediate.cert.pem certs/azure-iot-test-only.root.ca.cert.pem > certs/ca-full-chain.cert.pem

# Upload ca-full-chain.cert.pem to Azure DPS
## >> [Below command gave me an error, so I manually upload ca-full-chain.cert.pem to Portal]
## >> az iot dps certificate create --dps-name michi-dps-20190726 --resource-group michi-dps-20190726-rg --name dps20190726chain --path ./certs/ca-full-chain.cert.pem

# Manually upload ca-ful-chain.cert.pem to DPS and name it dps20190726chain

# Get Latest ETAG
az iot dps certificate show -n dps20190726chain --dps-name michi-dps-20190726 -g michi-dps-20190726-rg

# Generate Validation Code for Chain CA (2EF194C10D480915EBB4DAD7B3A18CCDEECF793D1FFEAFB6)
az iot dps certificate generate-verification-code -g michi-dps-20190726-rg --dps-name michi-dps-20190726 -n dps20190726chain -e <ETAG>

# Create verification certificate
./certGenEx.sh  create_intermediate_verification_certificate <VERIFICATION_CODE>

# Get Latest ETAG (AAAAAAD8c2k=)
az iot dps certificate show -n dps20190726chain --dps-name michi-dps-20190726 -g michi-dps-20190726-rg

# Verify Chain CA
az iot dps certificate verify --dps-name michi-dps-20190726 -g michi-dps-rg --name dps20190726chain --path ./certs/verification-code.cert.pem -e <ETAG>

## Create Enrollment Group with GeoLatency allocation policy
az iot dps enrollment-group create --dps-name michi-dps-20190726 -g michi-dps-20190726-rg --enrollment-id dpseastasia --root-ca-name dps20190726chain --ap geolatency
```

- Now that we have our Root Certificate and Intermediate Certificate ready. We use them to generate device certificates

```bash
# Create Device CA
rm ./certs/new-device.cert.pem
./certGen.sh create_device_certificate deviceid001

# Retrieve required information
az iot dps show -n michi-dps-20190726 -g michi-dps-20190726-rg

# A Json document should be returned, note down idScope value and deviceProvisioningHostName value
{
  "etag": "xxxxxxxxxxxxxx",
  "id": "/subscriptions/xxxxxxxxxxxxx/resourceGroups/michi-dps-20190726-rg/providers/Microsoft.Devices/provisioningServices/michi-dps-20190726",
  "location": "eastasia",
  "name": "michi-dps-20190726",
  "properties": {
    "allocationPolicy": "Hashed",
    "authorizationPolicies": null,
    "deviceProvisioningHostName": "global.azure-devices-provisioning.net",
    "idScope": "0neXXXXXXXX",
    "iotHubs": [
      {
        "allocationWeight": null,
        "applyAllocationPolicy": null,
        "connectionString": "HostName=michi-dps-eastasia-001.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=xxxxxxxxxxxxxxxxxxxxxxx",
        "location": "eastasia",
        "name": "michi-dps-eastasia-001.azure-devices.net"
      }
    ],
    "provisioningState": null,
    "serviceOperationsHostName": "michi-dps-20190726.azure-devices-provisioning.net",
    "state": "Active"
  },
  "resourcegroup": "michi-dps-20190726-rg",
  "sku": {
    "capacity": 1,
    "name": "S1",
    "tier": "Standard"
  },
  "subscriptionid": "xxxxxxxxxxxxxxxx",
  "tags": {},
  "type": "Microsoft.Devices/provisioningServices"
```

-  Provide Device certificates, its password (if any), ID_Scope and deviceProvisioningHostName retrieved from above to hardware manufacturer

## Hardware manufacturer Steps

As a hardware manufacturer, we need to safely stored device certificates to our device hardware for connectivity. This certificate also need to be updated periodically to ensure security.

In this tutorial, we will be using a simulator to simulate the device.

#### Codes

- Get [this sample code](https://github.com/Azure-Samples/azure-iot-samples-csharp/tree/master/provisioning/Samples/device/X509Sample)

- Update Program.cs, replace default ID_Scope, passowrd, certificate file name and deviceProvisioningHostName value

```csharp
private static string s_idScope = "0ne1234567";
//...
private static string s_certificateFileName = "new-device.cert.pfx";
//...
private static X509Certificate2 LoadProvisioningCertificate()
        {
            string certificatePassword = "1234"
//...
```

- Compile and run

```cshart
dotnet restore
dotnet build
dotnet run
```

- Device should provisioned successfully

```bash
Found certificate: AA7AE75A2BAD769079F021D388F58A9A40EA3C74 CN=deviceid001; PrivateKey: True
Using certificate AA7AE75A2BAD769079F021D388F58A9A40EA3C74 CN=deviceid001
RegistrationID = deviceid001
ProvisioningClient RegisterAsync . . . Assigned
ProvisioningClient AssignedHub: michi-dps-eastasia-001.azure-devices.net; DeviceID: deviceid001
Creating X509 DeviceClient authentication.
DeviceClient OpenAsync.
DeviceClient SendEventAsync.
DeviceClient CloseAsync.
```

## References
#### Group Registration
- [IoT Hub CA Certificate overview](https://github.com/Azure/azure-iot-sdk-c/blob/master/tools/CACertificates/CACertificateOverview.md)

- [Provision Device to IoT Hub tutorial](https://github.com/MicrosoftDocs/azure-docs/blob/master/articles/iot-dps/tutorial-provision-device-to-hub.md)

- [DPS - Setup Device Tutorial](https://github.com/MicrosoftDocs/azure-docs/blob/master/articles/iot-dps/tutorial-set-up-device.md)

- [DPS - Security](https://github.com/MicrosoftDocs/azure-docs/blob/master/articles/iot-dps/concepts-security.md#controlling-device-access-to-the-provisioning-service-with-x509-certificates)

- [X.509 Certificates and Enrollment Group explained](https://github.com/MicrosoftDocs/azure-docs/blob/master/articles/iot-dps/concepts-security.md#controlling-device-access-to-the-provisioning-service-with-x509-certificates)

- [Azure DPS Group Enrollments](https://docs.microsoft.com/zh-tw/azure/iot-dps/tutorial-group-enrollments)

- [x509 Client sample](https://docs.microsoft.com/zh-tw/azure/iot-dps/quick-create-simulated-device-x509-csharp)

#### Issue

- https://github.com/Azure/azure-iot-sdk-csharp/issues/1010

