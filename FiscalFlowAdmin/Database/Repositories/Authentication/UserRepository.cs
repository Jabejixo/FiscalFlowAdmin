using FiscalFlowAdmin.Helpers;
using FiscalFlowAdmin.Model;
using Microsoft.Extensions.Logging;

namespace FiscalFlowAdmin.Database.Repositories.Authentication;

public class UserRepository(FiscalFlowDatabaseContext context)
    : BaseRepository<User>(context)
{
    public bool Login(string email, string password)
    {
        var user = context.Set<User>().FirstOrDefault(u => u.Email == email);
        if (user != null)
        {
            // Используем метод VerifyPassword для проверки пароля, который извлекает всё необходимое из хеша
            return Hasher.VerifyPassword(password, user.Password);
        }
        return false;
    }
    public new async Task<bool> AddAsync(User? entity)
    {
        if (entity == null)
        {
            return false;
        }

        try
        {
            entity.Password = Hasher.SetPassword(entity.Password);
            await context.Set<User>().AddAsync(entity);
            await context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
    
}