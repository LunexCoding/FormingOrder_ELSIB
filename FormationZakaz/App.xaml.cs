using System;
using System.Reflection;
using System.Windows;
//
using Fox;
using FormationZakaz.ViewModels;
using FormationZakaz.Views;
//
namespace FormationZakaz
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    partial class App
    {
        /// <summary>
        /// Обработчик события при загрузке App
        /// </summary>
        /// <param name="_sender"></param>
        /// <param name="_e"></param>
        void App_OnStartup(object _sender, StartupEventArgs _e)
        {
            try
            {
                VMLocator.InitializeVMLocator(Assembly.GetExecutingAssembly());
                //
                if (CheckStartUp.CheckRun())
                {
                    string name = typeof(Main).Name;
                    int index = VMLocator.CreateViewModel(name);
                    ((Main)VMLocator.VMs[name][index].view).Loaded += ((vmMain)VMLocator.VMs[name][index]).viewLoaded;
                    VMLocator.VMs[name][index].view.Show();
                }
                else Shutdown();
            }
            catch (Exception _ex) { ExceptionHandler.ShowException(_ex, Current); }
        }
    }
}
