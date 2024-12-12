using FiscalFlowAdmin.Model;
using Microsoft.Extensions.Logging;

namespace FiscalFlowAdmin.Database.Repositories.Reminders;

public class NotificationProfileRepository(FiscalFlowDatabaseContext context) : 
    BaseRepository<NotificationProfile>(context)
{
    
}