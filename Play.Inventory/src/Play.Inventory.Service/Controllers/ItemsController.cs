using Microsoft.AspNetCore.Mvc;
using Play.Common;
using Play.Inventory.Service.Clients;
using Play.Inventory.Service.DTO;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service.Controllers;

[ApiController]
[Route("[controller]")]
public class ItemsController : ControllerBase
{
    private readonly IRepository<InventoryItem> _repository;
    private readonly CatalogClient _catalogClient;
    
    public ItemsController(IRepository<InventoryItem> repository, CatalogClient catalogClient)
    {
        _repository = repository;
        _catalogClient = catalogClient;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetAsync(Guid userId)
    {
        if (userId == Guid.Empty) return BadRequest();

        var catalogItems = await _catalogClient.GetCatalogItemsAsync();
        var inventoryItemsEntities = await _repository.GetAllAsync(item => item.UserId == userId);

        var inventoryItemDtos = inventoryItemsEntities.Select(inventoryItem =>
        {
            var catalogItem = catalogItems.Single(catalogItem => catalogItem.Id == inventoryItem.CatalogItemId);
            return inventoryItem.AsDto(catalogItem.Name, catalogItem.Description);
        });
        return Ok(inventoryItemDtos);
    }
    
    [HttpPost]
    public async Task<ActionResult> PostAsync(GrantItemsDto grantItemsDto)
    {
        var inventoryItem = await _repository.GetAsync(item => item.UserId == grantItemsDto.UserId
                                                               && item.CatalogItemId == grantItemsDto.CatalogItemId);

        if (inventoryItem is null)
        {
            inventoryItem = new InventoryItem
            {
                CatalogItemId = grantItemsDto.CatalogItemId,
                UserId = grantItemsDto.UserId,
                Quantity = grantItemsDto.Quantity,
                AcquiredDate = DateTimeOffset.UtcNow
            };

            await _repository.CreateAsync(inventoryItem);
        }
        else
        {
            inventoryItem.Quantity += grantItemsDto.Quantity;
            await _repository.UpdateAsync(inventoryItem);
        }

        return Ok();
    }
}