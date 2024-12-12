using FiscalFlowAdmin.Model;
using Microsoft.Extensions.Logging;

namespace FiscalFlowAdmin.Database.Repositories.Finances;

public class TransactionRepository(FiscalFlowDatabaseContext context)
    : BaseRepository<Transaction>(context)
{
    
}