using System;
using System.ServiceModel;

namespace ServiceHostConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //var baseAddress = DiscoveryHelper.AvailableIpcBaseAddress;
            //var host = new ServiceHost(typeof(UserManager), baseAddress);
            var host = new ServiceHost(typeof(UserManager));
            host.Open();

            Console.WriteLine("UserManager State = " + host.State.ToString());
            Console.ReadLine();

            Console.WriteLine("Closing...");
            host.Close();
        }
    }
}
