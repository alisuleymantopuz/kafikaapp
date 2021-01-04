using api.basket.Models;
using api.basket.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;

namespace api.basket.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BasketController : ControllerBase
    {
        private readonly BasketService _basketService;

        public BasketController(BasketService basketService)
        {
            _basketService = basketService;
        }

        [HttpGet]
        public ActionResult<List<Basket>> Get() => _basketService.Get();

        [HttpGet("{id:length(24)}", Name = "GetBasket")]
        public ActionResult<Basket> Get(string id)
        {
            var basket = _basketService.Get(id);

            if (basket == null)
            {
                return NotFound();
            }

            return basket;
        }

        [HttpGet("{basketKey}", Name = "GetBasketByBasketKey")]
        public ActionResult<Basket> GetBasketByBasketKey(string basketKey)
        {
            var basket = _basketService.GetByBasketKey(basketKey);

            if (basket == null)
            {
                return NotFound();
            }

            return basket;
        }

        [HttpPost]
        public ActionResult<Basket> Create(Basket basket)
        {
            _basketService.Create(basket);

            return CreatedAtRoute("GetBasket", new { id = basket.Id.ToString() }, basket);
        }

        [HttpPut]
        public ActionResult<Basket> Update(string basketKey, [FromBody] Basket basket)
        {
            _basketService.Update(basketKey, basket);

            return CreatedAtRoute("GetBasketByBasketKey", new { basketKey = basketKey }, basket);
        }

        [HttpDelete]
        public ActionResult<Basket> Delete(string basketKey)
        {
            _basketService.Remove(basketKey);

            return NoContent();
        }
    }
}
