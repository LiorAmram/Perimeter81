using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perimeter
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server();
            server.Start();
            CLI cli = new CLI(server);
            cli.Start();
        }
    }
}
