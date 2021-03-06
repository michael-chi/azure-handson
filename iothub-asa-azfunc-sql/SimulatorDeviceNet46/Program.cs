﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;

namespace Microsoft.Azure.Devices.Client.Samples
{
    class Program
    {
        // String containing Hostname, Device Id & Device Key in one of the following formats:
        //  "HostName=<iothub_host_name>;DeviceId=<device_id>;SharedAccessKey=<device_key>"
        //  "HostName=<iothub_host_name>;CredentialType=SharedAccessSignature;DeviceId=<device_id>;SharedAccessSignature=SharedAccessSignature sr=<iot_host>/devices/<device_id>&sig=<token>&se=<expiry_time>";
        // Either set the IOTHUB_DEVICE_CONN_STRING environment variable or within launchSettings.json:
        private static string DeviceConnectionString = "<DEVICE CONNECTION STRING>";

        private static int MESSAGE_COUNT = 5;
        private const int TEMPERATURE_THRESHOLD = 30;
        private static String deviceId = "itri001";
        private static float temperature;
        private static float humidity;
        private static Random rnd = new Random();

        static void Main(string[] args)
        {
            try
            {
                DeviceClient deviceClient = DeviceClient.CreateFromConnectionString(DeviceConnectionString, TransportType.Mqtt);

                deviceClient.OpenAsync().Wait();
                Task.Run(async () => SendEvent(deviceClient));//.Wait();
                Task.Run(async () => ReceiveCommands(deviceClient));//.Wait();

                Console.WriteLine("Exited!");
            }
            catch (AggregateException ex)
            {
                foreach (Exception exception in ex.InnerExceptions)
                {
                    Console.WriteLine();
                    Console.WriteLine("Error in sample: {0}", exception);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("Error in sample: {0}", ex.Message);
            }
            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();
        }

        static async Task SendEvent(DeviceClient deviceClient)
        {
            string dataBuffer;

            Console.WriteLine("Device sending {0} messages to IoTHub...\n", MESSAGE_COUNT);
            int count = 0;
            //for (int count = 0; count < MESSAGE_COUNT; count++)
            while(true)
            {
                temperature = rnd.Next(20, 35);
                humidity = rnd.Next(60, 80);
                dataBuffer = GetData(deviceId);// string.Format("{{\"deviceId\":\"{0}\",\"messageId\":{1},\"temperature\":{2},\"humidity\":{3}}}", deviceId, count, temperature, humidity);
                Message eventMessage = new Message(Encoding.UTF8.GetBytes(dataBuffer));
                eventMessage.Properties.Add("temperatureAlert", (temperature > TEMPERATURE_THRESHOLD) ? "true" : "false");
                Console.WriteLine("\t{0}> Sending message: {1}, Data: [{2}]", DateTime.Now.ToLocalTime(), count, dataBuffer);

                await deviceClient.SendEventAsync(eventMessage).ConfigureAwait(false);

                Task.Delay(3000).Wait();
            }
        }
        static string GetData(string deviceId)
        {
            if (rnd.Next(1, 100) >= 70) {
                var msg = new
                {
                    msgType = "insert",
                    deviceId = deviceId,
                    messageId = Guid.NewGuid().ToString("B"),
                    temperature = rnd.Next(20, 35),
                    humidity = rnd.Next(60, 80)
                };
                return JsonConvert.SerializeObject(msg);
            }
            else
            {
                var msg = new
                {
                    msgType = "update",
                    deviceId = deviceId,
                    messageId = Guid.NewGuid().ToString("B"),
                    temperature = rnd.Next(20, 35),
                    humidity = rnd.Next(60, 80)
                };
                return JsonConvert.SerializeObject(msg);
            }
        }
        static async Task ReceiveCommands(DeviceClient deviceClient)
        {
            Console.WriteLine("\nDevice waiting for commands from IoTHub...\n");
            Message receivedMessage = null;

            while (true)
            {
                try
                {
                    receivedMessage = await deviceClient.ReceiveAsync(TimeSpan.FromSeconds(1)).ConfigureAwait(false);

                    if (receivedMessage != null)
                    {
                        string messageData = Encoding.ASCII.GetString(receivedMessage.GetBytes());
                        Console.WriteLine("\t{0}> Received message: {1}", DateTime.Now.ToLocalTime(), messageData);

                        int propCount = 0;
                        foreach (var prop in receivedMessage.Properties)
                        {
                            Console.WriteLine("\t\tProperty[{0}> Key={1} : Value={2}", propCount++, prop.Key, prop.Value);
                        }

                        await deviceClient.CompleteAsync(receivedMessage).ConfigureAwait(false);
                    }
                }
                finally
                {
                    if (receivedMessage != null)
                    {
                        receivedMessage.Dispose();
                    }
                }
            }
        }
    }
}
