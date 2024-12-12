using FiscalFlowAdmin.Model;
using Microsoft.Extensions.Logging;

namespace FiscalFlowAdmin.Database.Repositories.Reminders;

public class NotificationRepository(FiscalFlowDatabaseContext context) : 
    BaseRepository<Notification>(context)
{
    
}