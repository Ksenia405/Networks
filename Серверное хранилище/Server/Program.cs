using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        async static Task<int> Main(string[] args)
        {
            info_for_server.info_for_server.Processor();
            ServerCloud server = new ServerCloud();
            await server.StartServer();

            return 1;
        }
    }
}
