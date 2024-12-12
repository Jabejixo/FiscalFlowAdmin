using System.ComponentModel.DataAnnotations.Schema;
using System.Windows;
using FiscalFlowAdmin.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;


namespace FiscalFlowAdmin.Database.Repositories;


public class BaseRepository<TEntity>(FiscalFlowDatabaseContext context)
    : IRepository<TEntity>
    where TEntity : Base
{
    
    private readonly FiscalFlowDatabaseContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private static readonly ILogger _logger = 
        LoggerFactory.Create(builder => builder.AddConsole())
            .CreateLogger(typeof(TEntity).Name);



    private void ValidateId(long id)
    {
        if (id <= 0)
        {
            throw new ArgumentException("ID должен быть больше нуля.", nameof(id));
        }
    }

    public async Task<TEntity> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        try
        {
            ValidateId(id);
            var entity = await _context.Set<TEntity>().FindAsync(new object[] { id }, cancellationToken).ConfigureAwait(false);
            if (entity == null)
            {
                _logger.LogWarning($"Сущность с ID {id} не найдена.");
                return null; // или можно использовать специальный объект
            }

            _logger.LogInformation($"Сущность с ID {id} успешно получена.");
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Произошла ошибка при получении сущности по ID.");
            return null;
        }
    }

    public async Task<TEntity?> GetByIdWithRelatedDataAsync(long id, CancellationToken cancellationToken = default)
    {
        try
        {
            ValidateId(id);
            var entityQuery = _context.Set<TEntity>().AsQueryable();

            var foreignKeyProperties = typeof(TEntity)
                .GetProperties()
                .Where(p => Attribute.IsDefined(p, typeof(ForeignKeyAttribute)))
                .ToList();

            foreach (var foreignKeyProperty in foreignKeyProperties)
            {
                entityQuery = entityQuery.Include(foreignKeyProperty.Name);
            }

            var entity = await entityQuery.SingleOrDefaultAsync(e => e.Id == id, cancellationToken).ConfigureAwait(false);
            if (entity == null)
            {
                _logger.LogWarning($"Сущность с ID {id} не найдена.");
                return null;
            }

            _logger.LogInformation($"Сущность с ID {id} успешно получена с связанными данными.");
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Произошла ошибка при получении сущности с связанными данными.");
            return null;
        }
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var entities = await _context.Set<TEntity>().ToListAsync(cancellationToken).ConfigureAwait(false);
            _logger.LogInformation("Все сущности успешно получены.");
            return entities;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Произошла ошибка при получении всех сущностей.");
            return Enumerable.Empty<TEntity>();
        }
    }

    public async Task<IEnumerable<TEntity>> GetAllWithRelatedDataAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var entityQuery = _context.Set<TEntity>().AsQueryable();

            var foreignKeyProperties = typeof(TEntity)
                .GetProperties()
                .Where(p => Attribute.IsDefined(p, typeof(ForeignKeyAttribute)))
                .ToList();

            foreach (var foreignKeyProperty in foreignKeyProperties)
            {
                entityQuery = entityQuery.Include(foreignKeyProperty.Name);
            }

            var entities = await entityQuery.ToListAsync(cancellationToken).ConfigureAwait(false);
            _logger.LogInformation("Все сущности успешно получены с связанными данными.");
            return entities;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Произошла ошибка при получении всех сущностей с связанными данными.");
            return Enumerable.Empty<TEntity>();
        }
    }

    public async Task<bool> AddAsync(TEntity? entity)
    {
        if (entity == null)
        {
            _logger.LogWarning("Сущность не может быть null.");
            return false;
        }

        try
        {
            await _context.Set<TEntity>().AddAsync(entity);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Сущность с ID {entity.Id} успешно добавлена.");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Произошла ошибка при добавлении сущности.");
            return false;
        }
    }

    public async Task<bool> UpdateAsync(TEntity? entity)
    {
        if (entity == null)
        {
            _logger.LogWarning("Сущность не может быть null.");
            return false;
        }

        try
        {
            var existingEntity = await _context.Set<TEntity>().FindAsync(entity.Id);
            if (existingEntity == null)
            {
                _logger.LogWarning($"Сущность с ID {entity.Id} не найдена.");
                return false;
            }

            _context.Entry(existingEntity).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Сущность с ID {entity.Id} успешно обновлена.");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Произошла ошибка при обновлении сущности.");
            return false;
        }
    }

    public async Task<bool> DeleteAsync(long id)
    {
        try
        {
            ValidateId(id);
            var entity = await _context.Set<TEntity>().FindAsync(id);
            if (entity == null)
            {
                _logger.LogWarning($"Сущность с ID {id} не найдена.");
                return false;
            }

            _context.Set<TEntity>().Remove(entity);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Сущность с ID {id} успешно удалена.");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Произошла ошибка при удалении сущности.");
            return false;
        }
    }

    public IQueryable<TEntity> GetQueryable()
    {
        return _context.Set<TEntity>().AsQueryable();
    }

    public async Task<bool> PartialUpdateAsync(long id, Action<TEntity> updateAction)
    {
        try
        {
            ValidateId(id);
            var existingEntity = await _context.Set<TEntity>().FindAsync(id);
            if (existingEntity == null)
            {
                _logger.LogWarning($"Сущность с ID {id} не найдена.");
                return false;
            }

            updateAction(existingEntity);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Частичное обновление сущности с ID {id} выполнено успешно.");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Произошла ошибка при частичном обновлении сущности.");
            return false;
        }
    }
}

