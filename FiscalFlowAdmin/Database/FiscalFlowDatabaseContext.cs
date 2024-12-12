using FiscalFlowAdmin.Model;
using Microsoft.EntityFrameworkCore;

namespace FiscalFlowAdmin.Database;

public partial class FiscalFlowDatabaseContext : DbContext
{
    public FiscalFlowDatabaseContext()
    {
    }

    public FiscalFlowDatabaseContext(DbContextOptions<FiscalFlowDatabaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ActionLog> ActionLogs { get; set; }
    
    public virtual DbSet<DeviceToken> AuthenticationDevicetokens { get; set; }

    public virtual DbSet<Profile> AuthenticationProfiles { get; set; }

    public virtual DbSet<User> AuthenticationUsers { get; set; }

    public virtual DbSet<Bill> FinancesAppBills { get; set; }

    public virtual DbSet<Credit> FinancesAppCredits { get; set; }

    public virtual DbSet<Currency> FinancesAppCurrencies { get; set; }

    public virtual DbSet<DailyCategoryExpense> FinancesAppDailycategoryexpenses { get; set; }

    public virtual DbSet<DailyReport> FinancesAppDailyreports { get; set; }

    public virtual DbSet<Debt> FinancesAppDebts { get; set; }

    public virtual DbSet<MonthlyExpense> FinancesAppMonthlyexpenses { get; set; }

    public virtual DbSet<Transaction> FinancesAppTransactions { get; set; }

    public virtual DbSet<TransactionCategory> FinancesAppTransactioncategories { get; set; }

    public virtual DbSet<Notification> RemindersNotifications { get; set; }

    public virtual DbSet<NotificationProfile> RemindersNotificationProfiles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
      => optionsBuilder.UseNpgsql("Host=127.0.0.1;Database=fiscal_flow;Username=fiscal_flow_superuser;Password=fiscAl_fLow#4134^54;Port=5432;");
}
