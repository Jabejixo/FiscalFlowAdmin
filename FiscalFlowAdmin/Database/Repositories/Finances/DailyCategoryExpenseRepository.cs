using FiscalFlowAdmin.Model;
using Microsoft.Extensions.Logging;

namespace FiscalFlowAdmin.Database.Repositories.Finances;

public class DailyCategoryExpenseRepository(
    FiscalFlowDatabaseContext context)
    : BaseRepository<DailyCategoryExpense>(context)
{
    
}