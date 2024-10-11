using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FrituurAllRelationships.Models;
using MVCp2Relatie.Data;
using Microsoft.AspNetCore.Authorization;

namespace FrituurAllRelationshipsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly FrituurDb _context;

        public ProductsController(FrituurDb context)
        {
            _context = context;
        }

        // GET: api/Products
        /// <summary>
        /// Get all products
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Returns the list of products</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _context.Products.ToListAsync();
        }

        /// <summary>
        /// Get a product by id
        /// </summary>
        /// <param name="id">The ID of the product to view</param>
        /// <returns></returns>
        /// <response code="200">Returns the requested product</response>
        /// <response code="404">If the product is not found</response>
        // GET: api/Products/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        /// <summary>
        /// Edit a product
        /// </summary>y
        /// <param name="id">The ID of the product to update</param>
        /// <param name="name">The new name of the product</param>
        /// <param name="price">The new price of the product</param>
        /// <returns></returns>
        /// <response code="204">If the update was successful</response>
        /// <response code="400">If the request is wrong</response>
        /// <response code="404">If the product is not found</response>
        // PUT: api/Products/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize]
        public async Task<IActionResult> PutProduct(int id, string name, decimal price)
        {

            if (string.IsNullOrWhiteSpace(name) || price < 0)
            {
                return BadRequest();
            }

            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }
            product.Name = name;
            product.Price = price;

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Add a product
        /// </summary>
        /// <param name="product">The product to be created</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     { 
        ///        (id can be deleted)
        ///        "name": "Item #1",
        ///        "price": 2.99
        ///     }
        ///
        /// </remarks>
        /// <response code="201">Returns the newly created item</response>
        /// <response code="400">If the request is wrong</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProduct", new { id = product.ProductId }, product);
        }


        // DELETE: api/Products/5
        /// <summary>
        /// Delete a product
        /// </summary>
        /// <param name="id">The ID of the product to Delete</param>
        /// <returns></returns>
        /// <response code="204">If the deletion was successful</response>
        /// <response code="404">If the product is not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
