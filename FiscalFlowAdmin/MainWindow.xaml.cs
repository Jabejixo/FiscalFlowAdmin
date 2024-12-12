using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FiscalFlowAdmin.Database;
using FiscalFlowAdmin.View.Windows;
using FiscalFlowAdmin.ViewModel;

namespace FiscalFlowAdmin;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private MainWindowViewModel _viewModel;
    public MainWindow()
    {
        InitializeComponent();
        _viewModel = new MainWindowViewModel();
        _viewModel.LoginAction += Login;
        this.DataContext = _viewModel;
    }

    private void Login()
    {
        var window = new AdminWindow();
        window.Show();
        this.Close();
    }

    private void AuthButton_OnClick(object sender, RoutedEventArgs e)
    {
        
    }
}