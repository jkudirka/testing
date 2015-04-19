using ServiceModelEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServiceHostConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var baseAddress = DiscoveryHelper.AvailableIpcBaseAddress;
            var host = new ServiceHost(typeof(UserManager), baseAddress);
            host.Open();

            //Can do blocking calls:
            Console.WriteLine("UserManager is ready to receive requests.");
            Console.ReadLine();

            host.Close();

        }
    }
}
