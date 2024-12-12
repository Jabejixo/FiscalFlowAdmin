using System.Diagnostics.Eventing.Reader;
using System.Windows;
using System.Windows.Input;
using FiscalFlowAdmin.Database;
using FiscalFlowAdmin.Database.Repositories.Authentication;
using FiscalFlowAdmin.Model;
using FiscalFlowAdmin.ViewModel.Commands;

namespace FiscalFlowAdmin.ViewModel;

public class MainWindowViewModel : ViewModelBase
{
    private DataManager _dataManager = new();
    private string _email = string.Empty;

    public string Email
    {
        get => _email;
        set
        {
            _email = value;
            OnPropertyChanged();
        }
    }
    private string _password = string.Empty;

    public string Password
    {
        get => _password;
        set
        {
            _password = value;
            OnPropertyChanged();
        }
    }

    private bool _isLogin { get; set; } = false;
    
    private RelayCommand _loginCommand;
    public RelayCommand LoginCommand
    {
        get
        {
            _loginCommand = new RelayCommand(_ => Login());
            return _loginCommand;
        }
    }

    public event Action? LoginAction;
    private void Login()
    {
        var userRepository = _dataManager.Users as UserRepository;
        _isLogin = userRepository!.Login(Email, Password);
        if (_isLogin)
        {
            LoginAction?.Invoke();
        }
        else
        {
            MessageBox.Show("Login Failed");
        }
    }
}