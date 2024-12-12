using FiscalFlowAdmin.Model;
using Microsoft.Extensions.Logging;

namespace FiscalFlowAdmin.Database.Repositories.Finances;

public class CurrencyRepository(FiscalFlowDatabaseContext context)
    : BaseRepository<Currency>(context)
{
    
}