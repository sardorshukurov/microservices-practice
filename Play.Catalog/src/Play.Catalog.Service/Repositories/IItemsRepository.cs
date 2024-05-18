using Play.Catalog.Service.Entities;

namespace Play.Catalog.Service.Repositories;

public interface IItemsRepository
{
    Task CreateAsync(Item entity);
    Task<IReadOnlyCollection<Item>> GetAllAsync();
    Task<Item?> GetAsync(Guid id);
    Task UpdateASync(Item entity);
    Task RemoveAsync(Guid id);
}