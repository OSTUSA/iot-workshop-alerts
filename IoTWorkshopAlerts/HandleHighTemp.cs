using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace IoTWorkshopAlerts
{
    public static class HandleHighTemp
    {
        private static ServiceClient _serviceClient = null;
        private static string _connectionString = null;

        [FunctionName("HandleHighTemp")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info( $"C# IoT Hub trigger function processed a message: {req}" );

            var rawContent = await req.Content.ReadAsStringAsync();
            log.Info( $"Given content: {rawContent}" );

            // Tell stream analytics that the batch size is too big. Will automatically adjust down
            // https://docs.microsoft.com/en-us/azure/stream-analytics/stream-analytics-with-azure-functions#create-a-function-in-azure-functions-that-can-write-data-to-azure-redis-cache
            if ( rawContent?.ToString().Length > 262144 )
            {
                log.Info( "Content too big:" + rawContent.ToString().Length );
                return new HttpResponseMessage( HttpStatusCode.RequestEntityTooLarge );
            }

            try
            {
                if ( _serviceClient == null )
                {
                    _connectionString = Environment.GetEnvironmentVariable( "IotHubConnectionString" );
                    _serviceClient = ServiceClient.CreateFromConnectionString( _connectionString );
                }

                var highTempDevices = await req.Content.ReadAsAsync<List<EventDTO>>();
                if( highTempDevices == null || highTempDevices.Count == 0 )
                {
                    log.Info( "No devices found in request." );
                    return req.CreateResponse( HttpStatusCode.OK );
                }
                log.Info( $"Have {highTempDevices.Count} devices to notify." );

                // Send a C2D message to each high temp device
                var tasks = new List<Task>();
                foreach( var highTempDevice in highTempDevices )
                {
                    var warning = new Warning( highTempDevice.DeviceId, highTempDevice.Temperature );
                    log.Info( $"Notifying: {warning.DeviceId}" );
                    tasks.Add( _serviceClient.SendAsync( highTempDevice.DeviceId, warning.ToMessage() ) );
                }
                await Task.WhenAll( tasks );

                log.Info( "All devices notified" );
                return req.CreateResponse( HttpStatusCode.OK );
            }
            catch ( Exception ex )
            {
                log.Warning( "Caught exception:" + ex.Message );

                // Return success anyway, since this is a demo and we don't want to get 
                //  a queue backed up, especially for simulated devices
                return req.CreateResponse( HttpStatusCode.OK );
            }
        }
    }
}
