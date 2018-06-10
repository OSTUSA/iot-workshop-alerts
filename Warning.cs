using Microsoft.Azure.Devices;
using Newtonsoft.Json;
using System.Text;

namespace OSTIoTWorkshop
{
    public class Warning
    {
        public string DeviceId { get; }
        public double CurrentTemp { get; }

        public Warning( string deviceId, double temperature )
        {
            DeviceId = deviceId;
            CurrentTemp = temperature;
        }

        public Message ToMessage()
        {
            var message = new Message( ToByteArray() );
            return message;
        }

        public byte[] ToByteArray()
        {
            var serialized = JsonConvert.SerializeObject( this );
            return Encoding.ASCII.GetBytes( serialized );
        }
    }
}
