using System.Windows;
using FiscalFlowAdmin.Database;
using FiscalFlowAdmin.Model;
using FiscalFlowAdmin.View.Controls;
using FiscalFlowAdmin.View.Pages;
using FiscalFlowAdmin.ViewModel;

namespace FiscalFlowAdmin.View.Windows;

public partial class AdminWindow : Window
{
    private readonly DataManager _dataManager;

    public AdminWindow()
    {
        InitializeComponent();
        _dataManager = new DataManager();
    }

    private void UserButton_Click(object sender, RoutedEventArgs e)
    {
        var userViewModel = new BaseViewModel<User>(_dataManager.Users);
        var entityManagementView = new EntityManagementView { DataContext = userViewModel };
        EntityManagementFrame.Content = entityManagementView;
    }

    private void ProfileButton_Click(object sender, RoutedEventArgs e)
    {
        var profileViewModel = new BaseViewModel<Profile>(_dataManager.Profiles);
        var entityManagementView = new EntityManagementView { DataContext = profileViewModel };
        EntityManagementFrame.Content = entityManagementView;
    }

    private void CurrencyButton_Click(object sender, RoutedEventArgs e)
    {
        var currencyViewModel = new BaseViewModel<Currency>(_dataManager.Currencies);
        var entityManagementView = new EntityManagementView { DataContext = currencyViewModel };
        EntityManagementFrame.Content = entityManagementView;
    }

    private void BillButton_Click(object sender, RoutedEventArgs e)
    {
        var viewModel = new BaseViewModel<Bill>(_dataManager.Bills);
        var entityManagementView = new EntityManagementView { DataContext = viewModel };
        EntityManagementFrame.Content = entityManagementView;
    }

    private void CreditButton_Click(object sender, RoutedEventArgs e)
    {
        var viewModel = new BaseViewModel<Credit>(_dataManager.Credits);
        var entityManagementView = new EntityManagementView { DataContext = viewModel };
        EntityManagementFrame.Content = entityManagementView;
    }

    private void DailyCategoryExpenseButton_Click(object sender, RoutedEventArgs e)
    {
        var viewModel = new BaseViewModel<DailyCategoryExpense>(_dataManager.DailyCategoryExpenses);
        var entityManagementView = new EntityManagementView { DataContext = viewModel };
        EntityManagementFrame.Content = entityManagementView;
    }

    private void DailyReportButton_Click(object sender, RoutedEventArgs e)
    {
        var viewModel = new BaseViewModel<DailyReport>(_dataManager.DailyReports);
        var entityManagementView = new EntityManagementView { DataContext = viewModel };
        EntityManagementFrame.Content = entityManagementView;
    }

    private void DebtButton_Click(object sender, RoutedEventArgs e)
    {
        var viewModel = new BaseViewModel<Debt>(_dataManager.Debts);
        var entityManagementView = new EntityManagementView { DataContext = viewModel };
        EntityManagementFrame.Content = entityManagementView;
    }

    private void MonthlyExpenseButton_Click(object sender, RoutedEventArgs e)
    {
        var viewModel = new BaseViewModel<MonthlyExpense>(_dataManager.MonthlyExpenses);
        var entityManagementView = new EntityManagementView { DataContext = viewModel };
        EntityManagementFrame.Content = entityManagementView;
    }

    private void NotificationButton_Click(object sender, RoutedEventArgs e)
    {
        var viewModel = new BaseViewModel<Notification>(_dataManager.Notifications);
        var entityManagementView = new EntityManagementView { DataContext = viewModel };
        EntityManagementFrame.Content = entityManagementView;
    }

    private void TransactionButton_Click(object sender, RoutedEventArgs e)
    {
        var viewModel = new BaseViewModel<Transaction>(_dataManager.Transactions);
        var entityManagementView = new EntityManagementView { DataContext = viewModel };
        EntityManagementFrame.Content = entityManagementView;
    }

    private void AdminPage_ButtonClick(object sender, RoutedEventArgs e)
    {
        EntityManagementFrame.Content = new AdminPage();
    }

    private void TransactionCategoryButton_Click(object sender, RoutedEventArgs e)
    {
        var viewModel = new BaseViewModel<TransactionCategory>(_dataManager.TransactionCategories);
        var entityManagementView = new EntityManagementView { DataContext = viewModel };
        EntityManagementFrame.Content = entityManagementView;
    }

    private void LogPage_ButtonClick(object sender, RoutedEventArgs e)
    {
        var viewModel = new BaseViewModel<ActionLog>(_dataManager.ActionLogs, false);
        var entityManagementView = new EntityManagementView { DataContext = viewModel };
        EntityManagementFrame.Content = entityManagementView;
    }
}