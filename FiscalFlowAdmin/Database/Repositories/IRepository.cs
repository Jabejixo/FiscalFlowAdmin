using FiscalFlowAdmin.Model;

namespace FiscalFlowAdmin.Database.Repositories;

public interface IRepository<TEntity> where TEntity : Base
{
    Task<TEntity> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<TEntity?> GetByIdWithRelatedDataAsync(long id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> GetAllWithRelatedDataAsync(CancellationToken cancellationToken = default);
    Task<bool> AddAsync(TEntity? entity);
    Task<bool> UpdateAsync(TEntity? entity);
    Task<bool> DeleteAsync(long id);
    IQueryable<TEntity> GetQueryable(); // Добавлен метод для получения IQueryable<TEntity>
    Task<bool> PartialUpdateAsync(long id, Action<TEntity> updateAction); // Метод для частичного обновления
}
