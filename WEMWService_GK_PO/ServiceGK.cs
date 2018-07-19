using ImportTools;
using System;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace WEMWService_GK_PO
{
    public partial class ServiceGK : ImportBase
    {
        public ServiceGK()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
        }

        protected override void OnStop()
        {
        }
    }
}
