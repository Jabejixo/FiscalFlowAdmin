using FiscalFlowAdmin.Model;
using Microsoft.Extensions.Logging;

namespace FiscalFlowAdmin.Database.Repositories.Finances;

public class CreditRepository(FiscalFlowDatabaseContext context)
    : BaseRepository<Credit>(context)
{
    
}