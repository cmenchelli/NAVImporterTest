using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace WEMWService_ArrowFtpFiles
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            if (Environment.UserInteractive)
            {
                ServiceArrowFtp service3 = new ServiceArrowFtp();
                service3.TestStartupAndStop(args);
            }
            else
            {   // Put the body of your old Main method here.
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] 
                        { 
                            new ServiceArrowFtp() 
                        };
                ServiceBase.Run(ServicesToRun);
            }
        }
    }
}