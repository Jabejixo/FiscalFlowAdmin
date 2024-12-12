using FiscalFlowAdmin.Model;
using Microsoft.Extensions.Logging;

namespace FiscalFlowAdmin.Database.Repositories.Finances;

public class DailyReportRepository(FiscalFlowDatabaseContext context)
    : BaseRepository<DailyReport>(context);