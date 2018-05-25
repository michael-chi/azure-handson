Overview
========

In this document we will be demonstrating how to protect your API behind Azure
API Management by Azure AD OAuth 2.0. We assume you have basic Azure knowledge
and understand how to create an API App on Azure.

There are several ways to protect my backend API. Microsoft official
[document](https://docs.microsoft.com/en-us/azure/api-management/api-management-howto-protect-backend-with-aad)
site has full explanation on each methods including connect to a virtual
network. In this document we are to archive below architecture. Assume we have a
backend API that is accessible for consumer applications only through API
Management (APIM). We want to protect our backend API via Azure AD, application
can only access the API through APIM with a valid OAuth 2.0 token.

![](media/bbc20c1cea9909cc35fdf57e22118baa.png)

To further protect your backend API, consider integrate with Virtual Network so
it only accessible from virtual network. For more information regarding Virtual
Network integration, please refer to this document:
<https://docs.microsoft.com/en-us/azure/api-management/api-management-using-with-vnet>

Reference
=========

-   Sample client application codes:
    <https://docs.microsoft.com/en-us/azure/active-directory/develop/active-directory-v2-code-samples>

-   APIM policy:
    <https://docs.microsoft.com/zh-tw/azure/api-management/api-management-access-restriction-policies#ValidateJWT>

-   Protect you backend API with Azure AD:
    <https://docs.microsoft.com/en-us/azure/api-management/api-management-howto-protect-backend-with-aad>

Create an API App
=================

In this section we will be creating a sample REST API on Azure API App. Source
codes are here. Download it and deploy to Azure API App via Visual Studio or
your favorite deployment tools. At this point you do not have to configure Azure
AD authentication as we will first verify everything works well without
authentication.

As a demo, I am using this API app here:
<https://michi-itri-demo-api.azurewebsites.net> which provides several methods

![](media/751cc137212c600fbc4bb587250696b9.png)

To simplify API Management setup, remember to enable swagger in your code.

![](media/2aeaf2ffdaebcde414f11baf3a00f699.png)

1.  Goto to retrieve full swagger document, copy the content, we will need it
    later.

![](media/9b9c871970ed92ce17112eb0b74da134.png)

Create API Management Instance
==============================

2.  Follow general Azure service creation steps to create an API Management
    service instance

![](media/d6b76511d7aa2c242855a5b01c81c2ac.png)

3.  Once created, go to APIs section

![](media/f5f269dd705c3a231f0c40062dcbe51c.png)

4.  Create a new API from API App in the right panel

![](media/068c12ac0ad5caf311166bddd7f7b1f4.png)

5.  Select your API App from the list

![](media/1a1276a50c6d99a25354c2a063c0d47f.png)

6.  You can specify Tags or Products here or leave it empty for now

![](media/4e4b5bc851ab2c48bce3b3acb5b0b930.png)

7.  Click “Create” to create APIM instance

8.  Goto the API we created above

![](media/ce22cfdd0f013a713214e7033dd80fd8.png)

9.  We want to edit Swagger definition so that APIM knows the full schema of our
    backed API

![](media/b930c44c0e85b7a3b061e9aa700e7d11.png)

10.  Paste swagger content we copied from

11.  Click “Save” to save the definition

![](media/316038306b7c638f879308321a83a186.png)

12.  Switch to “Setting” section, check everything configured correctly. By
    default you may see “Undefined” appears in “API URL Suffix” field, empty it.

![](media/bdffbf4ebc4418246da19673774490a5.png)

13.  Click “Save” to save all changes

14.  At this point we should have basic configuration setup. In latersteps we
    will be adding Azure AD authentication to APIM and our backend API (the API
    app)

15.  Use Postman to invoke APIM endpoint as well as API App endpoint to verify
    both endpoints work well.

![](media/d26bcdbe2bcbbf8ed9e269f44dc8d6ff.png)

Configure Azure AD Applications
===============================

In this section we will create two Azure AD applications. One represents the
backend API (the API App), the other represents the client application (the API
consumer). The consumer application credential will be used to consume backend
API.

16.  Goto Azure portal, open up Azure AD console

![](media/ec5343350b977c25126a8b7de56f5078.png)

17.  Go to Application Registration

![](media/14426ae77edf2c50ecaedb8c6003dc43.png)

18.  Add a new application registration

![](media/53b673627f0f1b0767f358bc2bbf0f9b.png)

19.  Here we will be creating backend application to represents backend API

    -   Name: can be any friend name

    -   Application Type: Web app/API

    -   Sign-on Url: The URL for users to sign in to this API. We don’t really
        need this one but you have to specify a valid URI here.

![](media/ca1ea2c43858947b58fe3abd53619bdd.png)

20.  Once created, you should see overview like below

![](media/725efca81789901a40468633b8c66d8a.png)

21.  Repeat above steps to create client application (in this document,
    michi-ITRI-API-ClientApp)

22.  Once created **client application**, click Settings

![](media/018ac4e83332c77235854ce579f2a595.png)

23.  Here we will allow client application to access our backend application.
    Click “Required Permissions”

![](media/21ac628391ecbfb196c3de369ef06d06.png)

24.  Click Add to add a new application

![](media/6c6eaf23d343c8763dc658f0798025c0.png)

25.  Select backend application

![](media/c06bb888121f2c0e9063714a4e2427fd.png)

26.  Save changes, then click “Grant Permissions”

![](media/6e8d7fabbf4e8bcd5a0e491b591ed7e9.png)

27.  Grant access to backend API and click “Save” to save changes

![](media/cd2b2da6df0044c750c2ccb5522d807d.png)

28.  Here we are to generate an client secret for client application to
    authenticated. Goto “Keys”

![](media/149c3257bca36f1db24e27beb8dcace2.png)

29.  Give it a name, a duration then click “Save”

![](media/d95e55609fa8527b1037cb8dedb28867.png)

30.  Upon saving, the Key will be shown in the text field, note down the key, it
    won’t show up again

![](media/d705a1243222085a3ffac232bba616ad.png)

31.  Go to Properties tab, note down Application ID and APP ID URL

![](media/27b567ee5f67a520b055658ec64b45a1.png)

-   The Application ID here will be used as Client ID in later consumer codes

-   The App ID URI here will be used as Resource ID in later consumer codes

-   The key we note down in above step will be used as Client Secret in later
    consumer codes

32.  At this point we have all Azure AD authentication pre-requirements ready.
    Now we are to configure our backend API App to be authenticated by Azure AD.

33.  Open up API app console

34.  To to “Authentication/Authorization” section, enable “App Service
    Authentication” and specify “Login with Azure Active Directory”

![](media/b42052f227255637069b1b8b2781ff1c.png)

35.  Click Azure AD to configure Azure AD

![](media/f38c35d967eb6028694f01af4fc4a44a.png)

36.  Choose Advanced mode

![](media/cb1dfe24cf3190952fef1abdd1470902.png)

37.  Fill in required fields

    -   Client ID: Client application’s application Id we created above

    -   Client Secret: Client applications’ key we created above

    -   Allowed toke audiences: Is the receipt of the JWT token, which is the
        client application’s resource URI we copied above

![](media/f42b2e5d963cead073f483ca17552e90.png)

38.  Click “OK” to save changes

39.  Click “Save” to save all changes

![](media/fb2491e341f4fbb436cda0fb377f02a9.png)

40.  Now we have backend API protected by Azure AD.

41.  Do Postman, you should now requested to login

![](media/5bde1fc8e5357f7c9744e01d50ba15e5.png)

Configure Azure AD for APIM
===========================

In this section we will configure Azure API Management with Azure AD to protect
our backend API. Full document is here:
<https://docs.microsoft.com/en-us/azure/api-management/api-management-howto-protect-backend-with-aad>

42.  Goto Azure Portal, open Azure Activity Directory console and switch to the
    Azure AD you used above

43.  Click Endpoints

![](media/43351fd66833675882549c388e668abf.png)

44.  Note down Authorization endpoint and Token endpoint.

![](media/9c0b2e3c52b41f0b8ecd79f14d5b0be1.png)

45.  Token endpoint (and Authorization endpoint) are in below format:

>   [https://login.microsoftonline.com/\<AAD](https://login.microsoftonline.com/%3cAAD)
>   Tenant ID\>/oauth2/token

>   Note down AAD Tenant ID as well, we will need them later.

46.  Now open up again the backend AAD application we registered above.

![](media/7ea4e63ee5dbc54cd6b3897027d6108e.png)

47.  Note down application ID. This is the resource Id we will be configure to
    APIM later.

![](media/dc51d46ce8f9b114fdab993849ff047f.png)

48.  Goto Azure Portal, open APIM management console

49.  Click OAuth 2.0 then Add. This will bring up a OAuth 2.0 form.

![](media/5df7fe04fdb76037e8628fa8da49691c.png)

50.  Fill-in required information

    -   **Client registration page URL**: If your application has a registration
        page, enter the URL here. Otherwise give it a placeholder such as
        <https://localhost>

    -   **Authorization grant types:** Authentication Code

![](media/05f5f0898aecb9dddcad43b3b26870b0.png)

-   **Authorization endpoint URL & Token endpoint URL**: Fill in the
    authorization endpoint URL and Token endpoint URL we noted above.

![](media/dc7e51e178f8becfd12601358c65d836.png)

-   **Additional Body parameter:**

    -   Name: resource

    -   Value: the backend resource Id we retrieved earlier.

![](media/9ec57a3b94c0d979455df33c8b62fd64.png)

-   Client Credential:

    -   **Client ID:** The application Id of Azure AD Client Application we
        created in Azure AD

    -   **Client Secret:** The Key we generated for Azure AD client application

![](media/1d06bb4d72b09d1e7659ddb48ad67a84.png)

-   Note down the following redirect_url

![](media/350c52d74d4e34f9b133b3f001b8ce16.png)

-   Click “Save”

-   Goto your Client Azure AD application page

![](media/5f7f80fef2dd2a27fad2855ab9cc733e.png)

-   Click Settings, Then Redirect URL, paste the URL we copied above

![](media/4009b2edaef034a6f6b047c3035dc3dc.png)

-   Click Properties, Note down the APP ID URI

![](media/d6ae2efde33a46bc7a584130baea290d.png)

51.  Now goto APIM console, Click APIs then the API we created above

![](media/9aa35ff42ef639485c875aab9d93f5c8.png)

52.  Under Security, check OAuth 2.0 then select the OAuth 2.0 mechanism we
    created above and then “Save” to save changes.

![](media/7317e96bbf14f04fd0188574fc086e61.png)

53.  At this point we have all OAuth 2.0 configuration ready

Create Client Application
=========================

Now we are ready to create our client application to consume backend API via
APIM

54.  Goto APIM console, Open Developer Portal

![](media/6d5ee6396d00c800c0745b729d9d1499.png)

55.  Click the Backend API

![](media/2d88b9c9fd6e57c9016edf37f8f5d24a.png)

56.  Choose an API, click “Try It”

![](media/aa674e33655c3952e51f408b1b0cf6fb.png)

57.  Copy the subscription Key

![](media/3d8db28d0730b5963d2e4b7dfbb244e9.png)

58.  To back to APIM console, Click the backend API, then Settings. Copy the base
    URL

![](media/ee077dc123dce24c2a88413b77716e31.png)

59.  Create a console application in Visual Studio, and install latest version of
    Microsoft.IdentityModel.Clients.ActiveDirectory package.

![](media/ccf7aca40152d24afc9e92a8e0af6646.png)

60.  Create below method to retrieve OAuth 2.0 token from Azure AD
```csharp
protected static string GetAuthorizationHeader()
{
    AuthenticationResult result = null;
    var context = new AuthenticationContext("\<AUTHORIZATION URL\>");
    var thread = new Thread(() =>
    {
        var clientResourceUri = "\<AAD CLIENT APP RESOURCE URL\>";
        var clientCred = new ClientCredential("\<CLIENT APP ID\>", "\<CLIENT APP KEY\>");
        result = Task.Run(() => context.AcquireTokenAsync(clientResourceUri,clientCred)).Result;
    });
    thread.SetApartmentState(ApartmentState.STA);
    thread.Name = "AquireTokenThread";
    thread.Start();
    thread.Join();
    if (result == null)
    {
        throw new InvalidOperationException("Failed to obtain the JWT token");
    }
    string token = result.AccessToken;
    return token;
}
```
-   AUTHORIZATOIN URL: The endpoint URL we retrieve at Step 44.
-   AAD CLIENT APP RESOURCE URL:The Client APP ID URI at Step 31.
-   CLIENT APP ID: The Client Application ID we retrieve at Step 31
-   CLIENT APP KEY: Key generated at Step 2

61.  Add below codes to Main()
```csharp
static void Main(string[] args)
{
    var token = GetAuthorizationHeader();
    var url = "https://\<BASE URL\>/api/Values";
    var req = HttpWebRequest.Create(url) as HttpWebRequest;
    req.Headers.Add("Authorization", "Bearer " + token);
    req.Headers.Add("Ocp-Apim-Subscription-Key", "\<SUBSCRIPTION KEY\>");
    req.Method = "GET";
    using (var respStream = req.GetResponse().GetResponseStream())
    {
        using (var sr = new StreamReader(respStream))
        {
            var text = sr.ReadToEnd();
            Console.WriteLine(text);
            Console.ReadKey();
        }
    }
}
```
-   SUBSCRITION KEY: Key rettieve at Step 57
-   BASE URL: APIM Base URL at Step 58
62. Run the application to see results.