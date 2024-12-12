using FiscalFlowAdmin.Model;
using Microsoft.Extensions.Logging;

namespace FiscalFlowAdmin.Database.Repositories.Finances;

public class TransactionCategoryRepository(FiscalFlowDatabaseContext context) : BaseRepository<TransactionCategory>(context)
{
    
}