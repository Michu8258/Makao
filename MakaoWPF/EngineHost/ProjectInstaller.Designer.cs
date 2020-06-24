namespace EngineHost
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.MakaoGameEngineHostProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.MakaoGameEngineHostInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // MakaoGameEngineHostProcessInstaller
            // 
            this.MakaoGameEngineHostProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.MakaoGameEngineHostProcessInstaller.Password = null;
            this.MakaoGameEngineHostProcessInstaller.Username = null;
            this.MakaoGameEngineHostProcessInstaller.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.MakaoGameEngineHostProcessInstaller_AfterInstall);
            // 
            // MakaoGameEngineHostInstaller
            // 
            this.MakaoGameEngineHostInstaller.Description = "Makao Game Host service is the service that allows to play makao card game. It is" +
    " workin on host player\'s PC, and is hosting the game state.";
            this.MakaoGameEngineHostInstaller.DisplayName = "MakaoGameHostService via Windows Service";
            this.MakaoGameEngineHostInstaller.ServiceName = "MakaoGameHostService";
            this.MakaoGameEngineHostInstaller.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.MakaoGameEngineHostInstaller_AfterInstall);
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.MakaoGameEngineHostProcessInstaller,
            this.MakaoGameEngineHostInstaller});

        }

        #endregion

        public System.ServiceProcess.ServiceProcessInstaller MakaoGameEngineHostProcessInstaller;
        public System.ServiceProcess.ServiceInstaller MakaoGameEngineHostInstaller;
    }
}