using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using ShoppingService.Core.Cart;
using LanguageExt;
using static LanguageExt.Prelude;

namespace ShoppingService.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/cart")]
    [ApiController]
    public class CartController : ControllerBase
    {
       private readonly ICartService _service;
       public CartController(ICartService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CartItem>>> Get() =>
            await match(_service.GetItemsFromCart(),
                Right: items => Ok(items),
                Left: error => StatusCode(error.StatusCode, error.Messages)
            );

        [HttpGet("{id}")]
        public async Task<ActionResult<CartItem>> GetById(Guid id) =>
            await match(_service.GetItemById(id),
                Right: item => Ok(item),
                Left: error => StatusCode(error.StatusCode, error.Messages)
            );

        [HttpPost]
        public async Task<ActionResult<CartItem>> Post([FromBody] CartItem newItem) =>
            await match(_service.AddItemToCart(newItem),
                Right: item => StatusCode(201, item),
                Left: error => StatusCode(error.StatusCode, error.Messages)
            );

        [HttpPut("{id}")]
        public async Task<ActionResult<CartItem>> Put(Guid id, [FromBody] CartItem updatedItem) =>
            await match(_service.UpdateItemInCart(updatedItem),
                Right: item => Ok(item),
                Left: error => StatusCode(error.StatusCode, error.Messages)
            );

        [HttpDelete("{id}")]
        public async Task<ActionResult<Guid>> Delete(Guid id) =>
            await match(_service.RemoveItemFromCart(id),
                Right: removedId => StatusCode(200, removedId),
                Left: error => StatusCode(error.StatusCode, error.Messages)
            );
    }
}
