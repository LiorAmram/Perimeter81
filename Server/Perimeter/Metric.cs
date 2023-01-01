using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace Perimeter
{
    class Metric
    {
        DateTime timestamp;
        float temperature;

        public DateTime Timestamp { get => timestamp; }
        public float Temperature { get => temperature; }

        public Metric(DateTime timestamp, float temperature)
        {
            this.timestamp = timestamp;
            this.temperature = temperature;
        }

        public static Metric FromJsonText(string jsonText)
        {
            try
            {
                var jobject = JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JObject>(jsonText);
                DateTime timestamp = DateTime.Parse(jobject["timestamp"].ToString());
                float temperature = float.Parse(jobject["temperature"].ToString());
                return new Metric(timestamp, temperature);
            }
            catch
            {
                return null;
            }

            
        }
    }
}
