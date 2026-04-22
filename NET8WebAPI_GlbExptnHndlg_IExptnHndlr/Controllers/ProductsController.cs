using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NET8WebAPI_GlbExptnHndlg_IExptnHndlr.DTOs;
using NET8WebAPI_GlbExptnHndlg_IExptnHndlr.GlobleExptnHandlers.CustomizedExceptions;
using NET8WebAPI_GlbExptnHndlg_IExptnHndlr.Models;

namespace NET8WebAPI_GlbExptnHndlg_IExptnHndlr.Controllers
{
    //Global Exptn Doc: https://codewithmukesh.com/blog/global-exception-handling-in-aspnet-core/

    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly TestDbContext _testDbContext;

        public ProductsController(TestDbContext testDbContext)
        {
            _testDbContext = testDbContext;
        }

        [HttpGet("GetAllProducts")]
        public async Task<ActionResult<IEnumerable<Product>>> GetAllProducts()
        {
            return await _testDbContext.Products.ToListAsync();
        }


        [HttpGet("GetProductById/id")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductById(int id)
        {
            var product = await _testDbContext.Products.FindAsync(id);

            if (product is null)
            {
                throw new NotFoundException("Product", id);
            }

            return Ok(product);
        }

        [HttpPost("CreateProduct")]
        public async Task<IActionResult> CreateProduct([FromBody] Product product)
        {
            if (product is null)
            {
                throw new BadRequestException("No Product was specified.");
            }
            
            _testDbContext.Products.Add(product);
            await _testDbContext.SaveChangesAsync();

            return Ok(CreatedAtAction("GetProductById", new { id = product.Id}, product));
        }

        [HttpPut("UpdateProduct")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductDto productDto)
        {
            if (productDto is null)
            {
                throw new BadRequestException("No Id were given.");
            }

            var productToUpdate = await _testDbContext.Products.FindAsync(id);

            if (productToUpdate is null)
            {
                throw new NotFoundException("Product with Id was not found.", id);
            }

            productToUpdate.Brand = productDto.Brand;
            productToUpdate.Name = productDto.Name!;
            productToUpdate.Description = productDto.Description!;

            _testDbContext.Products.Update(productToUpdate);
            await _testDbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("DeleteProduct/id")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (id == 0)
            {
                throw new BadRequestException("No Product was specified.");
            }

            var productToRemove = _testDbContext.Products.Find(id);
            if (productToRemove is null)
            {
                throw new NotFoundException("No Product found by this Product with Id.", id);
            }

            _testDbContext.Products.Remove(productToRemove);

            await _testDbContext.SaveChangesAsync();

            return Ok();
        }

    }
}
