using api.products.persistence.Context;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace api.products.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private ProductsRepositoryContext _context;
        public ProductsController(IMapper mapper, ProductsRepositoryContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        [HttpGet("GetProducts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
            return Ok(_context.Products.ToList());
        }
    }
}
