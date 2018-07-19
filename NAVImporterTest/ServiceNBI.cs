using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAVImporterTest
{
    public partial class ServiceNBI : Component
    {
        public ServiceNBI()
        {
            InitializeComponent();
        }

        public ServiceNBI(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
