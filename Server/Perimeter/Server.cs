using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Perimeter
{
    class Server
    {
        public const int NUMBER_OF_SENSORS = 100;
        SensorHandler[] sensors;
        HttpListener listener;
        string url = "http://localhost:30000/";
        bool active;
        Queue<HttpListenerContext> pendingRequests;

        public Server()
        {
            sensors = new SensorHandler[NUMBER_OF_SENSORS];
            for (int i=0; i< sensors.Length; i++)
            {
                sensors[i] = new SensorHandler(i);
            }
            pendingRequests = new Queue<HttpListenerContext>();
        }

        public void Start()
        {
            active = true;
            listener = new HttpListener();
            listener.Prefixes.Add(url);
            listener.Start();
            Task.Factory.StartNew(handle);
            Task.Factory.StartNew(handleRequests);
        }

        public void Stop()
        {
            active = false;
        }

        public float GetMaxTemperature(int id, DateTime date)
        {
            if (id < sensors.Length)
            {
                return sensors[id].GetMaxTemperature(date);
            }
            return float.NaN;
        }

        public float GetMaxTemperature(int id)
        {
            if (id < sensors.Length)
            {
                return sensors[id].GetMaxTemperature();
            }
            return float.NaN;
        }

        public float GetMaxTemperature()
        {
            float max = float.MinValue;
            float currentMax;
            bool maxUpdated = false;
            for (int i = 0; i < sensors.Length; i++)
            {
                currentMax = GetMaxTemperature(i);
                if (!float.IsNaN(currentMax) && currentMax > max)
                {
                    maxUpdated = true;
                    max = currentMax;
                }
            }
            return (maxUpdated) ? max : float.NaN;
        }

        public float GetMinTemperature(int id, DateTime date)
        {
            if (id < sensors.Length)
            {
                return sensors[id].GetMinTemperature(date);
            }
            return float.NaN;
        }

        public float GetMinTemperature(int id)
        {
            if (id < sensors.Length)
            {
                return sensors[id].GetMinTemperature();
            }
            return float.NaN;
        }

        public float GetMinTemperature()
        {
            float min = float.MaxValue;
            float currentMin;
            bool minUpdated = false;
            for (int i = 0; i < sensors.Length; i++)
            {
                currentMin = GetMinTemperature(i);
                if (!float.IsNaN(currentMin) && currentMin < min)
                {
                    minUpdated = true;
                    min = currentMin;
                }
            }
            return (minUpdated) ? min : float.NaN;
        }

        public float GetAverageTemperature(int id, DateTime date)
        {
            if (id < sensors.Length)
            {
                return sensors[id].GetAverageTemperature(date);
            }
            return float.NaN;
        }

        public float GetAverageTemperature(int id)
        {
            if (id < sensors.Length)
            {
                return sensors[id].GetAverageTemperature();
            }
            return float.NaN;
        }

        public float GetAverageTemperature()
        {
            float sum = 0;
            float currentAvg;
            int count = 0;
            for (int i = 0; i < sensors.Length; i++)
            {
                currentAvg = GetAverageTemperature(i);
                if (!float.IsNaN(currentAvg))
                {
                    sum += currentAvg;
                    count++;
                }
            }
            return (count > 0) ? (sum / count) : float.NaN;
        }

        private void handle()
        {
            HttpListenerContext ctx;
            while (active)
            {
                ctx = listener.GetContext();

                // Peel out the requests and response objects
                pendingRequests.Enqueue(ctx);
            }
        }

        private HttpStatusCode validateRequest(HttpListenerRequest req) 
        {
            if (req.HttpMethod  != "POST")
            {
                return HttpStatusCode.MethodNotAllowed;
            }

            if (req.Url.LocalPath != "/update")
            {
                return HttpStatusCode.NotFound;
            }

            return HttpStatusCode.OK;
        }

        private void handleRequests() 
        {
            HttpListenerContext currentRequest;
            int sensorId;
            while (active || pendingRequests.Count > 0)
            {
                if (pendingRequests.Count > 0)
                {
                    currentRequest = pendingRequests.Dequeue();
                    HttpListenerRequest req;
                    HttpListenerResponse resp;

                    req = currentRequest.Request;
                    resp = currentRequest.Response;

                    HttpStatusCode respStatusCode = validateRequest(req);
                    resp.StatusCode = (int)respStatusCode;

                    if (respStatusCode != HttpStatusCode.OK) // valid request
                    {
                        resp.Close();
                        continue;
                    }

                    sensorId = int.Parse(req.Headers.Get("sensor-id"));
                    if (sensorId > NUMBER_OF_SENSORS - 1)
                    {
                        resp.StatusCode = (int)HttpStatusCode.BadRequest;
                        resp.Close();
                        continue;
                    }
                    
                    var body = new StreamReader(req.InputStream).ReadToEnd();
                    Metric metric = Metric.FromJsonText(body);
                    if (metric == null)
                    {
                        resp.StatusCode = (int)HttpStatusCode.BadRequest;
                        resp.Close();
                        continue;
                    }
                    sensors[sensorId].AddMetric(metric);

                    resp.Close();
                }
            }
        }
    }
}
