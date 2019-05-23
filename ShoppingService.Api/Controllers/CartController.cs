using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ShoppingService.Core.Cart;

namespace ShoppingService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _service;

        public CartController(ICartService service)
        {
            _service = service;
        }

        // GET api/shopping/cart
        [HttpGet]
        // [ProducesResponseType(typeof(IEnumerable<CartItem>), Status200OK)]
        public async Task<ActionResult<IEnumerable<CartItem>>> Get()
        {
            var items = await _service.GetAllItems();
            return Ok(items);
        }

        // GET api/shopping/cart/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CartItem>> GetById(Guid id)
        {
            var item = await _service.GetById(id);

            if (item == null) {
                return NotFound();
            }

            return Ok(item);
        }

        // POST api/shopping/cart
        [HttpPost]
        public async Task<ActionResult<CartItem>> Post([FromBody] CartItem value)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var item = await _service.Add(value);
            return CreatedAtAction(nameof(Get), new { id = item.Id }, item);
        }

        // PUT api/shopping/cart/5
        [HttpPut("{id}")]
        public void Put(Guid id, [FromBody] string value)
        {

        }

        // DELETE api/shopping/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Guid>> Delete(Guid id)
        {
            var existingItem = await _service.GetById(id);
            if (existingItem == null) {
                return NotFound();
            }

            var removedGuid = _service.Remove(id);
            return Ok(removedGuid);
        }
    }
}
