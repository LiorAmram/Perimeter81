using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perimeter
{
    class MetricsDay
    {
        List<Metric> metrics;
        float min;
        float max;
        float average;

        public float Min { get => min; }
        public float Max { get => max; }
        public float Average { get => average; }

        public MetricsDay()
        {
            this.metrics = new List<Metric>();
            this.min = float.MaxValue;
            this.max = float.MinValue;
            this.average = 0;
        }

        public void AddMetric(Metric metric)
        {
            this.metrics.Add(metric);
            if (metric.Temperature > this.max)
            {
                this.max = metric.Temperature;
            }

            if (metric.Temperature < this.min)
            {
                this.min = metric.Temperature;
            }

            this.average = (this.average * (metrics.Count - 1) + metric.Temperature) / metrics.Count;
        }
    }
}
