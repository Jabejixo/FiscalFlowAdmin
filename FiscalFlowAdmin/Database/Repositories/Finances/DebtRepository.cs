using FiscalFlowAdmin.Model;
using Microsoft.Extensions.Logging;

namespace FiscalFlowAdmin.Database.Repositories.Finances;

public class DebtRepository(FiscalFlowDatabaseContext context)
    : BaseRepository<Debt>(context);
    