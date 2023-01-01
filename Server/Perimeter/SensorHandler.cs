using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perimeter
{
    class SensorHandler
    {
        private readonly int sensorID;
        private Dictionary<int, MetricsDay> metrics;
        
        public int SensorID { get => sensorID;}

        public SensorHandler(int sensorID)
        {
            this.sensorID = sensorID;
            metrics = new Dictionary<int, MetricsDay>();
        }

        public void AddMetric(Metric metric)
        {
            int dateHash = Utils.HashDate(metric.Timestamp);
            if (!metrics.ContainsKey(dateHash))
            {
                metrics[dateHash] = new MetricsDay();
                cleanOldData();
            }
            metrics[dateHash].AddMetric(metric);
        }
        
        public float GetMaxTemperature(DateTime date)
        {
            MetricsDay metricsDay = getMetricsByDate(date);
            if (metricsDay != null)
            {
                return metricsDay.Max;
            }
            return float.NaN;
        }

        public float GetMaxTemperature()
        {
            var metrics = GetMetricAggragation(7, GetMaxTemperature);
            return (metrics.Count > 0) ? metrics.Max() : float.NaN;
        }

        public float GetMinTemperature(DateTime date)
        {
            MetricsDay metricsDay = getMetricsByDate(date);
            if (metricsDay != null)
            {
                return metricsDay.Min;
            }
            return float.NaN;
        }

        public float GetMinTemperature()
        {
            var metrics = GetMetricAggragation(7, GetMinTemperature);
            return (metrics.Count > 0) ? metrics.Min() : float.NaN;
        }

        public float GetAverageTemperature(DateTime date)
        {
            MetricsDay metricsDay = getMetricsByDate(date);
            if (metricsDay != null)
            {
                return metricsDay.Average;
            }
            return float.NaN;
        }

        public float GetAverageTemperature()
        {
            var metrics = GetMetricAggragation(7, GetAverageTemperature);
            return (metrics.Count > 0) ? metrics.Average() : float.NaN;
        }

        private MetricsDay getMetricsByDate(DateTime date)
        {
            int dateHash = Utils.HashDate(date);
            if (metrics.ContainsKey(dateHash))
            {
                return metrics[dateHash];
            }
            return null;
        }

        private List<float> GetMetricAggragation(int days, Func<DateTime, float> method)
        {
            List<float> metrics = new List<float>();
            float currentMetric;
            for (int i = 0; i < days; i++)
            {
                currentMetric = method(DateTime.Now.AddDays(-1 * i));
                if (!float.IsNaN(currentMetric))
                {
                    metrics.Add(currentMetric);
                }
            }

            return metrics;
        }

        private void cleanOldData()
        {
            // Simplest implementation. It can be more robust. I assumed there is always continuous data
            if (metrics.Count > 7)
            {
                metrics.Remove(Utils.HashDate(DateTime.Now.AddDays(-7).Date));
            }
        }

    }
}
