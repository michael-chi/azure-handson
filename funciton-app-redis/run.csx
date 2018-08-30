
#r "Newtonsoft.Json"

using System.Net;
using StackExchange.Redis;
using System.Net.Http;
using Newtonsoft.Json;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info("C# HTTP trigger function processed a request.");

    var connString = System.Configuration.ConfigurationManager.AppSettings["REDIS_CONNECTION_STRING"];
    //var redis = ConnectionMultiplexer.Connect(connString);
    var redis = ConnectionMultiplexer.Connect(connString).GetDatabase();
    string name = req.GetQueryNameValuePairs()
            .FirstOrDefault(q => string.Compare(q.Key, "name", true) == 0)
            .Value;
    log.Info($"name={name}");
    if(name == null){
        log.Info($"name is null");
        return req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a name on the query string or in the request body");
    }
    else if(req.Method == HttpMethod.Post){
        log.Info($"Post");
        //we want to add item to cache
        dynamic data = await req.Content.ReadAsAsync<object>();
        log.Info($"data={JsonConvert.SerializeObject(data)}");
        redis.StringSet(name, JsonConvert.SerializeObject(data));
        return req.CreateResponse(HttpStatusCode.OK, new {status = "OK"});
    }else if(req.Method == HttpMethod.Get){
        //we want to get cache item from cache
        log.Info($"Get");
        var data = JsonConvert.DeserializeObject(redis.StringGet(name));
        return req.CreateResponse(HttpStatusCode.OK, data);
    }else{
        return req.CreateResponse(HttpStatusCode.BadRequest, new {Status="Error",Message="Unsupported Method"});
    }
    // parse query parameter
    /*
    if (name == null)
    {
        // Get request body
        dynamic data = await req.Content.ReadAsAsync<object>();
        name = data?.name;
    }

    return name == null
        ? req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a name on the query string or in the request body")
        : req.CreateResponse(HttpStatusCode.OK, "Hello " + name);
    */
}
