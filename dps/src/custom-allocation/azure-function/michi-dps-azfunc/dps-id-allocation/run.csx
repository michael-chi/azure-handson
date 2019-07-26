#r "Newtonsoft.Json"

using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System.IO;

public static async Task<IActionResult> Run(HttpRequest req, ILogger log)
{
    // Just some diagnostic logging
    log.LogInformation("C# HTTP trigger function processed a request.");
    log.LogInformation("Request.Content:...");    
    
    // Get request body
    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
    log.LogInformation(requestBody);
    dynamic data = JsonConvert.DeserializeObject(requestBody);

    // Get registration ID of the device
    string regId = data?.deviceRuntimeContext?.registrationId;

    string message = "Uncaught error";
    bool fail = false;
    ResponseObj obj = new ResponseObj();

    if (regId == null)
    {
        message = "Registration ID not provided for the device.";
        log.LogInformation("Registration ID : NULL");
        fail = true;
    }
    else
    {
        string[] hubs = data?.linkedHubs.ToObject<string[]>();

        // Must have hubs selected on the enrollment
        if (hubs == null)
        {
            message = "No hub group defined for the enrollment.";
            log.LogInformation("linkedHubs : NULL");
            fail = true;
        }
        else
        {
            //  TODO: replace below with allocation logic
            obj.iotHubHostName = hubs[1];
        }
    }
    return (!fail)
        ? (ActionResult)new OkObjectResult(obj)
        : new BadRequestObjectResult(message);
    
}

public class DeviceTwinObj
{
    public string deviceId {get; set;}
}

public class ResponseObj
{
    public string iotHubHostName {get; set;}
    public string IoTHub {get; set;}
    public DeviceTwinObj initialTwin {get; set;}
    public string[] linkedHubs {get; set;}
    public string enrollment {get; set;}
}