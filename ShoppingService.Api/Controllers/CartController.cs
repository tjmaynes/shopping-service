using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using ShoppingService.Core.Cart;
using ShoppingService.Core.Common;
using LanguageExt;
using static LanguageExt.Prelude;

namespace ShoppingService.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/shopping/cart")]
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
                Right: result => Ok(new Dictionary<string, PagedResult<CartItem>> {{ "data", result }}),
                Left: error => StatusCode(ConvertErrorCode(error.ErrorCode), error.Message)
            );

        [HttpGet("{id}")]
        public async Task<ActionResult<CartItem>> GetById(string id) =>
            await match(_service.GetItemById(id),
                Right: item => Ok(new Dictionary<string, CartItem> {{ "data", item }}),
                Left: error => StatusCode(ConvertErrorCode(error.ErrorCode), error.Message)
            );

        [HttpPost]
        public async Task<ActionResult<CartItem>> Post([FromBody] CartItem newItem) =>
            await match(_service.AddItemToCart(newItem),
                Right: item => StatusCode(201, new Dictionary<string, CartItem> {{ "data", item }}),
                Left: error => StatusCode(ConvertErrorCode(error.ErrorCode), error.Message)
            );

        [HttpPut("{id}")]
        public async Task<ActionResult<CartItem>> Put(string id, [FromBody] CartItem updatedItem) =>
            await match(_service.UpdateItemInCart(updatedItem),
                Right: item => Ok(new Dictionary<string, CartItem> {{ "data", item }}),
                Left: error => StatusCode(ConvertErrorCode(error.ErrorCode), error.Message)
            );

        [HttpDelete("{id}")]
        public async Task<ActionResult<Guid>> Delete(string id) =>
            await match(_service.RemoveItemFromCart(id),
                Right: removedId => Ok(new Dictionary<string, string> {{ "data", id }}),
                Left: error => StatusCode(ConvertErrorCode(error.ErrorCode), error.Message)
            );

        private int ConvertErrorCode(ServiceErrorCode errorCode) {
            switch (errorCode) {
                case ServiceErrorCode.InvalidItem: return 422;
                case ServiceErrorCode.ItemNotFound: return 404;
                default: return 500;
            }
        }
    }
}
