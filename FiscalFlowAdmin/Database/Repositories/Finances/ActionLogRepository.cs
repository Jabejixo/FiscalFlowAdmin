using FiscalFlowAdmin.Model;

namespace FiscalFlowAdmin.Database.Repositories.Finances;

public class ActionLogRepository(FiscalFlowDatabaseContext context)
    : BaseRepository<ActionLog>(context)
{
    
}