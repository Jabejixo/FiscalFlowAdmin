using FiscalFlowAdmin.Model;
using Microsoft.Extensions.Logging;

namespace FiscalFlowAdmin.Database.Repositories.Authentication;

public class DeviceTokenRepository(FiscalFlowDatabaseContext context)
    : BaseRepository<DeviceToken>(context)
{
    
}