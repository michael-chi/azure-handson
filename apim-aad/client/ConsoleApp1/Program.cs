using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace ConsoleApp1
{
    class Program
    {
        protected static string GetAuthorizationHeader()
        {
            AuthenticationResult result = null;
            var context = new AuthenticationContext(
                "https://login.microsoftonline.com/<AAD Tenant ID>/oauth2/authorize");
            var thread = new Thread(() =>
            {
                var clientResourceUri = "https://microsoft.onmicrosoft.com/<ID>";
                var clientCred = new ClientCredential("<AAD Client APPLICATION ID>", "<AAD CLIENt KEY>");
                result = Task.Run(() => context.AcquireTokenAsync(clientResourceUri,
                                   clientCred)).Result;
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
        static void Main(string[] args)
        {
            var token = GetAuthorizationHeader();
            var url = "https://michi-itri-apimgnt.azure-api.net/api/Values";
            var req = HttpWebRequest.Create(url) as HttpWebRequest;
            req.Headers.Add("Authorization", "Bearer " + token);
            req.Headers.Add("Ocp-Apim-Subscription-Key", "<APIM Subscription Key>");
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
    }
}
