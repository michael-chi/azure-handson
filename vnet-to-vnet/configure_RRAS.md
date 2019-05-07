## Configure RRAS

### Enable RRAS Role

RDP into the Windows 2019 machine we created. Open up Server Manager and add Remote Access Roles with Direct Access and VPN (RAS) and Routing services.

![AddRRASRole](media/add_rras_role.jpg)

Once installed, open up RRAS management console via Server Manager

![RRASConsole](media/open_rras_console.jpg)

### Configure RRAS

-   Right click the local server, Configure and Enable Routing and Remote Access

![EnableRRAS](media/enable_rras.jpg)

-   Choose Custom Configuration and then check VPN and LAN Routing, wait until service started

![SecureConnection](media/rras_vpn_and_lanrouting.jpg)

-   Right click on Interface, New Demand-Dial Interface

![NewDemendDialInterface](media/rras_new_interface.jpg)

-   Name this new interface as AzureS2S

![AzureS2S](media/rras_new_interface.jpg)

-   Connect using VPN

![ConnectUsingVPN](media/rras_connect_using_vpn.jpg)

-   Use IKEv2, than Next

![IKEv2](media/rras_ikev2.jpg)

-   Specify destination IP address here, this should be the Cloud-GW IP address

![CloudGWIP](media/rras_remote_ip.jpg)

    You can get your cloud gateway from Azure Portal.

![CloudGW](media/rras_where_is_cloudgw.jpg)

![CloudGWIP2](media/rras_where_is_cloudgw_ip.jpg)

-   check Route Package on this interface

![RoutePackage](media/rras_route_package.jpg)

-   Add a static route, the CIDR should be your Cloud Virtual Network's address space. Add than Next.

![RRASDestinationIPAddressSpace](media/rras_add_static_route.jpg)

-   Fill in "Azure" as User name, leave other fields empty, click Next and finish this wizard.

![RRASUserName](media/rras_user_name.jpg)

-   Go to Interface, right click on AzureS2S interface, Properties than Security. Check Use Preshared key for authentication, fill in the key we generated previously here.

![RRAS_KEY](media/rras_update_interface_sharedkey.jpg)

-   Right click AzureS2S interface than click Connect, wait a few seconds you should see status become "Connected"

![RRASConnected](media/rras_connected.jpg)

### Create Custom Route Table

-   In order to have local machine communicate to Azure Machines, create a new Route as below

![Route](media/vnet_create_route.jpg)

-   Associate it with your local data subnet

![Route2](media/vnet_associate_local_route.jpg)