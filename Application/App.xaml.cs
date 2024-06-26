﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MainApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            //ConfigureContainer();
            ServiceLocator.RegisterSingleton<InspectService>(() => new InspectService());
            //RunApplication();
        }

        private void RunApplication()
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }
}
