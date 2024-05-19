using Microsoft.AspNetCore.Mvc;
using Play.Common;
using Play.Inventory.Service.DTO;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service.Controllers;

[ApiController]
[Route("[controller]")]
public class ItemsController : ControllerBase
{
    private readonly IRepository<InventoryItem> _repository;

    public ItemsController(IRepository<InventoryItem> repository)
    {
        _repository = repository;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetAsync(Guid userId)
    {
        if (userId == Guid.Empty) return BadRequest();

        var items = (await _repository.GetAllAsync(item => item.UserId == userId))
            .Select(item => item.AsDto());

        return Ok(items);
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