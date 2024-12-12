using FiscalFlowAdmin.Model;
using Microsoft.Extensions.Logging;

namespace FiscalFlowAdmin.Database.Repositories.Finances;

public class BillRepository(FiscalFlowDatabaseContext context)
    : BaseRepository<Bill>(context)
{
    
}