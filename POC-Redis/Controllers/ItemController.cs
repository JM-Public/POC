using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

using Newtonsoft.Json;

using POC_Redis.Models;

namespace POC_Redis.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemController(ILogger<ItemController> logger, IDistributedCache cache) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(Item), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IActionResult), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetItemsAsync(string itemID)
        {
            var cached = await cache.GetStringAsync(itemID);

            if (String.IsNullOrEmpty(cached))
            {
                return NotFound();
            }

            logger.LogInformation("Successfully retrieved item from cache");

            return Ok(JsonConvert.DeserializeObject<Item>(cached));
        }

        [HttpPut]
        [ProducesResponseType(typeof(IActionResult), StatusCodes.Status204NoContent)]
        public async Task<IActionResult> PutItemAsync(string itemID, string itemName)
        {
            var item = new Item() { Id = itemID, Name = itemName };

            await cache.SetStringAsync(itemID, JsonConvert.SerializeObject(item));

            return NoContent();
        }
    }
}
