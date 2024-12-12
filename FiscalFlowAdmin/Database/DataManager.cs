using FiscalFlowAdmin.Database.Repositories;
using FiscalFlowAdmin.Database.Repositories.Authentication;
using FiscalFlowAdmin.Database.Repositories.Finances;
using FiscalFlowAdmin.Database.Repositories.Reminders;
using FiscalFlowAdmin.Model;
using Microsoft.EntityFrameworkCore;

namespace FiscalFlowAdmin.Database;

public class DataManager : IDisposable
{
    private readonly FiscalFlowDatabaseContext _context;
    
    public IRepository<DeviceToken> DeviceTokens { get; private set; }
    public IRepository<User> Users { get; private set; }
    public IRepository<Profile> Profiles { get; private set; }
    public IRepository<Bill> Bills { get; private set; }
    public IRepository<Credit> Credits { get; private set; }
    public IRepository<Currency> Currencies { get; private set; }
    public IRepository<DailyCategoryExpense> DailyCategoryExpenses { get; private set; }
    public IRepository<DailyReport> DailyReports { get; private set; }
    public IRepository<Debt> Debts { get; private set; }
    public IRepository<MonthlyExpense> MonthlyExpenses { get; private set; }
    public IRepository<TransactionCategory> TransactionCategories { get; private set; }
    public IRepository<Transaction> Transactions { get; private set; }
    public IRepository<Notification> Notifications { get; private set; }
    public IRepository<NotificationProfile> NotificationProfiles { get; private set; }
    
    public IRepository<ActionLog> ActionLogs { get; private set; }

    public DataManager()
    {
        _context = new FiscalFlowDatabaseContext();
        //Инициализация репозиториев
        ActionLogs = new ActionLogRepository(_context);
        DeviceTokens = new DeviceTokenRepository(_context);
        Users = new UserRepository(_context);
        Profiles = new ProfileRepository(_context);
        Bills = new BillRepository(_context);
        Credits = new CreditRepository(_context);
        Currencies = new CurrencyRepository(_context);
        DailyCategoryExpenses = new DailyCategoryExpenseRepository(_context);
        DailyReports = new DailyReportRepository(_context);
        Debts = new DebtRepository(_context);
        MonthlyExpenses = new MonthlyExpenseRepository(_context);
        TransactionCategories = new TransactionCategoryRepository(_context);
        Transactions = new TransactionRepository(_context);
        Notifications = new NotificationRepository(_context);
        NotificationProfiles = new NotificationProfileRepository(_context);
    }
    public void Save() => _context.SaveChanges();
    public async ValueTask DisposeAsync()
    {
        if (_context.Database.GetDbConnection().State == System.Data.ConnectionState.Open)
        {
            await _context.DisposeAsync();
        }
    }

    public void Dispose()
    {
        DisposeAsync().AsTask().Wait();
    }

    
}