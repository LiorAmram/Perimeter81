using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perimeter
{
    static class Utils
    {
        public static int HashDate(DateTime date)
        {
            return date.Date.GetHashCode();
        }
    }
}
