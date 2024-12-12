using System.Configuration;
using System.Data;
using System.Windows;
using FiscalFlowAdmin.Database;
using FiscalFlowAdmin.Database.Repositories;
using FiscalFlowAdmin.Database.Repositories.Authentication;
using FiscalFlowAdmin.Database.Repositories.Finances;
using FiscalFlowAdmin.Database.Repositories.Reminders;
using FiscalFlowAdmin.Model;
using FiscalFlowAdmin.ViewModel;
using Microsoft.Extensions.DependencyInjection;

namespace FiscalFlowAdmin;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
    {
        
        public App()
        {
   
        }

    }