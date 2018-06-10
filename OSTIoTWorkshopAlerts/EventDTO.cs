namespace OSTIoTWorkshopAlerts
{
    /// <summary>
    /// Defines event content structure
    /// </summary>
    public class EventDTO
    {
        public string DeviceId { get; set; }
        public double Temperature { get; set; }
        public double Humidity { get; set; }
    }
}
