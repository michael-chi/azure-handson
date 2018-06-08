#r "System.Configuration"
#r "System.Data"
#r "Newtonsoft.Json"
using System.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info("C# HTTP trigger function processed a request.");
    var str = "<SQL CONNECTION STRING>";
    dynamic data = await req.Content.ReadAsAsync<object>();
    log.Info(JsonConvert.SerializeObject(data));
    foreach(dynamic item in data){
        string deviceId = item?.deviceid;
        var temp = item?.temperature;
        var humidity = item?.humidity;
        using (SqlConnection conn = new SqlConnection(str))
        {
            conn.Open();
            var text = $"UPDATE UpdateData set Temperature={temp}, Humidity={humidity}, UpdateTime=getdate() where DeviceId='{deviceId}'";
            log.Info(text);
            using (SqlCommand cmd = new SqlCommand(text, conn))
            {
                // Execute the command and log the # rows affected.
                var rows = await cmd.ExecuteNonQueryAsync();
            }
        }
    }
    return req.CreateResponse(HttpStatusCode.OK,"OK");
}
