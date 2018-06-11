# IoT Workshop Alerts

Includes code that implements alert handlers as [Azure Functions](https://azure.microsoft.com/en-us/services/functions/) for the [IoT Workshop](https://www.ostusa.com/news-events/iot-developer-camp/) being held at Microsoft's Detroit MTC in late spring, 2018.

Note that this is sample code only, and hasn't been hardended for production use.

 ## Functions

 ### Handle High Temp

 The workshop involves working with an internet-connected device, with a set of sensors - specifically including a temparature sensor. This scenario explores how to handle a case where incoming Telemetry (device-to-cloud) messages indicate a higher-than-normal temperature, as might happen to field device experiencing adverse conditions.

There's currently one function implemented: `HandleHighTemp` (`HandleHighTemp.cs`), which exposes an HTTP trigger. The trigger is intended to be invoked by a Stream Analytics job, which owns the criteria for an "alerting" device. If an event reaches this handler, it is by definition an event worth alerting about, and this function handles the event by sending a cloud-to-device message back to the device that caused the alert - which the target device could take action on.

## Help

Contact [OST](https://www.ostusa.com/expertise/connected-products/) / [OpenDigital](https://www.opendigital.com) for assistance.