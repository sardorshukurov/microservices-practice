using Microsoft.AspNetCore.Mvc;

namespace Play.Catalog.Service.Controllers;

[ApiController]
[Route("[controller]")]
public class ItemsController : ControllerBase
{
    private static readonly List<ItemDto> Items = 
    [
        new ItemDto(Guid.NewGuid(), "Potion", "Restores a small amount of HP", 5, DateTimeOffset.UtcNow),
        new ItemDto(Guid.NewGuid(), "Antidote", "Cures poison", 7, DateTimeOffset.UtcNow),
        new ItemDto(Guid.NewGuid(), "Bronze sword", "Deals a small amount of damage", 20, DateTimeOffset.UtcNow),
    ];

    [HttpGet]
    public IEnumerable<ItemDto> Get()
    {
        return Items;
    }

    [HttpGet("{id}")]
    public ActionResult<ItemDto> GetById(Guid id)
    {
        ItemDto? item = Items.FirstOrDefault(i => i.Id == id);
        if (item is null) return NotFound();
        return item;
    }

    [HttpPost]
    public ActionResult<ItemDto> Post(CreateItemDto createItemDto)
    {
        var item = new ItemDto(Guid.NewGuid(), createItemDto.Name, createItemDto.Description, createItemDto.Price,
            DateTimeOffset.UtcNow);
        Items.Add(item);

        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    [HttpPut("{id}")]
    public IActionResult Put(Guid id, UpdateItemDto updateItemDto)
    {
        ItemDto? existingItem = Items.FirstOrDefault(i => i.Id == id);
        if (existingItem is null) return NotFound();

        ItemDto updatedItem = existingItem with
        {
            Name = updateItemDto.Name,
            Description = updateItemDto.Description,
            Price = updateItemDto.Price
        };

        var index = Items.FindIndex(i => i.Id == id);
        Items[index] = updatedItem;

        return NoContent();
    }

    [HttpDelete]
    public IActionResult Delete(Guid id)
    {
        var index = Items.FindIndex(i => i.Id == id);

        if (index < 0) return NotFound();
        
        Items.RemoveAt(index);
        
        return NoContent();
    }
}