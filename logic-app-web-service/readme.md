Overview
========

In this document I will be demonstrating a common scenario in EAI/B2B solutions
which is to receive JSON request in a Logic App Http Trigger, extract its fields
and invoke an external web service with those fields as parameters.

There are several ways to do this, for example, you can create an API Management
instance, APIM come with capability to import WSDL schema, so you can use it as
a proxy to talk to Web Service.

In this document, we use a public free web service as our invoking service, you
can replace it with your own web service.

The overall flow looks like below

![](media/1ce8861d2fca094346797089caa278cc.png)

Table of Content
================

Create Custom Connector
=======================

First we will be creating an “Adaptor” for our Logic App to connect with Web
Service. In order to do so, we will be creating a customer connector for our
Logic App

1.  Create a customer connector from your Azure Portal

![](media/ab866bd2e7e16b8cb453baaa550dccdf.png)

2.  Give it basic information and click “Create”

![](media/4cd00f6fca966554b4cdb72e5e20425b.png)

3.  Once created, click “Edit”

![](media/7043923ae4b22a482b576180ae260ced.png)

4.  In general tab, you want to switch API endpoint to SOAP (preview) and Call
    mode to “SOAP to REST”.

>   If you have WSDL locally you can upload it, otherwise you may want to fill
>   in WSDL URL and let Azure download it for you.

>   Once specified, click “Upload”

![](media/fbbe98d0190571fcc2788e2af87f2475.png)

5.  Scroll down, if you like you can give your connector an icon and description

![](media/3b1ef8196b47f13dfcc78955e1342e1b.png)

6.  Scroll down, specify HTTP or HTTPS here depending on actual requirement from
    your web service. Here I am using a free web service which allows HTTP. You
    should leave other fields as is.

>   Once finished, click “Security”

![](media/4eb3dd7b795b013720ab07f69c24c810.png)

7.  Here we allows only Basic Authentication. In my case I am using a free web
    service without authentication, hence leave it blank.

>   Click “Defination”

![](media/d516c7b652a853f8674907f384eeab3a.png)

8.  Next, we will fine tune our Web Service interface. In my testing web service
    it exposes 4 web methods – Add, Subtract, Divide, Multiply. You can review
    each method and fill in descriptions…etc. Here I want to use Add method
    only.

![](media/e49125d2a24b345e1efe613514752502.png)

9.  Once done, Click “Update Connector”

![](media/7c8b37946d6cea7759628a55f41d3f09.png)

Create Logic App
================

10.  Now go back to Azure Portal, create a Logic App

![](media/69d309f08a2a15b042464093f7113762.png)

11.  Open Logic App Editor, let’s start from the Http Template

![](media/c90fac087966bfec5904bd6391ac36f5.png)

12.  I want my HTTP trigger to accept JSON body and I want to simplify access to
    JSON body in my Logic App. So here I want to provide a sample JSON body so
    that Logic App knows how to extract fields from HTTP request body

>   In my case, I want to allow my HTTP client to submit a JSON body like below.
>   You may define your own JSON body schema
```json
>   {
   "arg0":3,
   "arg1":4
   }
```
13.  So I past my sample JSON to the editor. Click “Done” when completed.

![](media/1d120cbb605bc0910bb49d1e5f2ad3c8.png)

14.  Next, let’s add an Action to invoke Web Service

![](media/c6be165b17e1ddc89039f9b8153de4c1.png)

15.  Here let’s find our newly created custom connector

![](media/b5f4a1b20507b598762b522411b3a2d6.png)

16.  It automatically shows all web methods in the web service that is defined in
    our connector

![](media/897b9aa001f7f081f2d31a4d80f66557.png)

17.  Click the Add method

18.  Here it lists required parameters for “Add” method – intA and intB which is
    defined in the web service WSDL. In right panel, it shows us possible fields
    in HTTP Trigger’s JSON body that we can use (as we gave a sample JSON in
    above steps)

![](media/44122963e08349121971bbdf32f963fa.png)

19.  Place JSON fields in right panel to the connector parameters by click one of
    the fields listed in right panel.

![](media/2fbe1ed779fe828699d3e8c69296bb60.png)

20.  Click Next Step to add a new Action

![](media/e4d7e65056092fb2a764c30f02b00374.png)

21.  We want to send back HTTP response to our client application, let’s use
    Request – Response connector

![](media/c81e470a77768ebdeb1c456a5dbfdfbc.png)

22.  Specify Body as “addResponse” as we want to return “Add” method’s response
    to my client application.

![](media/22a72adc8433ac9d3e099012242b68df.png)

23.  Save the Logic App and go back to HTTP Trigger, copy the Logic App URL

![](media/7228009c4c2acbb79ec791fcd88fe514.png)

Verify
======

24.  Use your favorite HTTP testing tool to verify result

![](media/8188747257e4fb251bb94a55d6cf60ce.png)
