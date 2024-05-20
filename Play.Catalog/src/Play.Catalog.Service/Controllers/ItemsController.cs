using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Service.DTO;
using Play.Catalog.Service.Entities;
using Play.Common;

namespace Play.Catalog.Service.Controllers;

[ApiController]
[Route("[controller]")]
public class ItemsController : ControllerBase
{
    private readonly IRepository<Item> _repository;
    private static int _requestCounter = 0;
    
    public ItemsController(IRepository<Item> repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ItemDto>>> GetAsync()
    {
        _requestCounter++;
        Console.WriteLine($"Request {_requestCounter}: Starting...");

        if (_requestCounter <= 2)
        {
            Console.WriteLine($"Request {_requestCounter}: Delaying...");
            await Task.Delay(TimeSpan.FromSeconds(10));
        }
        
        if (_requestCounter <= 4)
        {
            Console.WriteLine($"Request {_requestCounter}: 500 (Internal Server Error).");
            return StatusCode(500);
        }
        
        var items = (await _repository.GetAllAsync())
                                            .Select(item => item.AsDto());
        
        Console.WriteLine($"Request {_requestCounter}: 200 (OK).");
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ItemDto>> GetByIdASync(Guid id)
    {
        var item = await _repository.GetAsync(id);
        if (item is null) return NotFound();
        return item.AsDto();
    }

    [HttpPost]
    public async Task<ActionResult<ItemDto>> PostAsync(CreateItemDto createItemDto)
    {
        var item = createItemDto.AsEntity();
        await _repository.CreateAsync(item);
        return CreatedAtAction(nameof(GetByIdASync), new { id = item.Id }, item);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutAsync(Guid id, UpdateItemDto updateItemDto)
    {
        var existingItem = await _repository.GetAsync(id);

        if (existingItem is null) return NotFound();

        existingItem.Name = updateItemDto.Name;
        existingItem.Description = updateItemDto.Description;
        existingItem.Price = updateItemDto.Price;

        await _repository.UpdateAsync(existingItem);
        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var existingItem = await _repository.GetAsync(id);

        if (existingItem is null) return NotFound();

        await _repository.RemoveAsync(existingItem.Id);
        
        return NoContent();
    }
}