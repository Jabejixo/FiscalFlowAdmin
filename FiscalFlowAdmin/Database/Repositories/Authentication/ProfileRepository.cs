using FiscalFlowAdmin.Model;
using Microsoft.Extensions.Logging;

namespace FiscalFlowAdmin.Database.Repositories.Authentication;

public class ProfileRepository(FiscalFlowDatabaseContext context)
    : BaseRepository<Profile>(context)
{
    
}