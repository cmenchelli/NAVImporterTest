using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace NISLateOrdersService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>        
        static void Main(string[] args)
        {
            if (Environment.UserInteractive)
            {
                ServiceNisLateOrders service18 = new ServiceNisLateOrders();
                service18.TestStartupAndStop(args);
            }
            else
            {
                // Put the body of your old Main method here.
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] 
                    { 
                        new ServiceNisLateOrders() 
                    };
                ServiceBase.Run(ServicesToRun);
            }
        }
    }
}
