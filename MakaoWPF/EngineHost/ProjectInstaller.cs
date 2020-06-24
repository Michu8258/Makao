using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;

namespace EngineHost
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        private void MakaoGameEngineHostInstaller_AfterInstall(object sender, InstallEventArgs e)
        {

        }

        private void MakaoGameEngineHostProcessInstaller_AfterInstall(object sender, InstallEventArgs e)
        {

        }
    }
}
