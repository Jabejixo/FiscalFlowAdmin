using FiscalFlowAdmin.Model;
using Microsoft.Extensions.Logging;

namespace FiscalFlowAdmin.Database.Repositories.Finances;

public class MonthlyExpenseRepository(FiscalFlowDatabaseContext context)
    : BaseRepository<MonthlyExpense>(context)
{
    
}