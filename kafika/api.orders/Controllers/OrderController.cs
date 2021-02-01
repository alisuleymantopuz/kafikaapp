using api.orders.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using api.orders.persistence.Messaging.Sender;
using api.orders.persistence.Context;
using api.orders.persistence.Entities;

namespace api.orders.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly OrdersRepositoryContext _context;
        private readonly IOrderCreateMessagingSender _orderCreateSender;
        private readonly IOrderShipperMessagingSender _orderShipperMessagingSender;
        private readonly IOrderDeliveryMessagingSender _orderDeliveryMessagingSender;
        public OrderController(IMapper mapper, OrdersRepositoryContext context, IOrderCreateMessagingSender orderCreateSender, IOrderShipperMessagingSender orderShipperMessagingSender, IOrderDeliveryMessagingSender orderDeliveryMessagingSender)
        {
            _mapper = mapper;
            _context = context;
            _orderCreateSender = orderCreateSender;
            _orderShipperMessagingSender = orderShipperMessagingSender;
            _orderDeliveryMessagingSender = orderDeliveryMessagingSender;

        }

        [HttpPost("Create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] OrderRequest model)
        {
            var order = _mapper.Map<Order>(model);
            order.GenerateNewId();
            order.UserId = Guid.Parse(User.Identity.Name);
            order.CreationDate = DateTime.UtcNow;
            order.OrderStatus = OrderStatus.Created;

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            _orderCreateSender.SendCreatedOrder(order);

            return Ok();
        }

        [HttpPost("Cancel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Cancel([FromBody] CancelOrder model)
        {
            var order = _context.Orders.FirstOrDefault(x => x.Id == Guid.Parse(model.OrderId));
            if (order == null)
                return NotFound();

            order.OrderStatus = OrderStatus.CancelRequested; 
            order.LastModifiedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            _orderCreateSender.SendCreatedOrder(order);

            return Ok();
        }

        [HttpPost("AssignShipper")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AssignShipper([FromBody] AssignShipperRequest model)
        {

            var order = _context.Orders.FirstOrDefault(x => x.Id == Guid.Parse(model.OrderId));
            if (order == null)
                return NotFound();

            order.ShipperName = model.ShipperName;
            order.ShipperPhone = model.ShipperPhone;
            order.LastModifiedDate = DateTime.UtcNow;
            order.ShippingAddress = model.ShippingAddress;
            order.OrderStatus = OrderStatus.ShipperAssigned;
            await _context.SaveChangesAsync();

            _orderShipperMessagingSender.SendShipperAssignedOrder(order);

            return Ok();
        }

        [HttpPost("DeliveryInProgress")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeliveryInProgress([FromBody] AddDeliveryNote model)
        {

            var order = _context.Orders.FirstOrDefault(x => x.Id == Guid.Parse(model.OrderId));
            if (order == null)
                return NotFound();

            order.OrderStatus = OrderStatus.DeliveryInProgress;
            order.OrderNotes += model.Notes;
            order.LastModifiedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            _orderDeliveryMessagingSender.SendDeliveryOrder(order);

            return Ok();
        }

        [HttpPost("Delivered")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delivered([FromBody] AddDeliveryNote model)
        {

            var order = _context.Orders.FirstOrDefault(x => x.Id == Guid.Parse(model.OrderId));
            if (order == null)
                return NotFound();

            order.OrderStatus = OrderStatus.Delivered;
            order.OrderNotes += model.Notes;
            order.LastModifiedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            _orderDeliveryMessagingSender.SendDeliveryOrder(order);

            return Ok();
        }

        [HttpGet("GetOrders")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
            var orders = await _context.Orders
                .Where(x => x.UserId == Guid.Parse(User.Identity.Name))
                .Include(x => x.OrderDetails)
                .ToListAsync();

            var orderModels = _mapper.Map<List<OrderModel>>(orders);

            return Ok(orderModels);
        }
    }
}
