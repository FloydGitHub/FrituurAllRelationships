using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FrituurAllRelationships.Models;
using MVCp2Relatie.Data;
using FrituurAllRelationships.ViewModels;
using System.Security.Claims;

namespace FrituurAllRelationships.Controllers
{
    public class OrdersController : Controller
    {
        private readonly FrituurDb _context;

        public OrdersController(FrituurDb context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var frituurDb = _context.Orders.Include(o => o.Customer)
        .Where(o => o.OrderDate.Date == DateTime.Today)
        .Where(o => o.Confirmed == false)
        .OrderByDescending(o => o.OrderDate);  // Sort op meest recente orders :)
            return View(await frituurDb.ToListAsync());
        }


        public async Task<IActionResult> ConfirmOrder(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            order.Confirmed = true;
            _context.Update(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> IndexConfirmed()
        {
            var frituurDb = _context.Orders.Include(o => o.Customer)
        .Where(o => o.OrderDate.Date == DateTime.Today)
        .Where(o => o.Confirmed == true)
        .OrderByDescending(o => o.OrderDate);  // Sort op meest recente orders :)
            return View(await frituurDb.ToListAsync());
        }




        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderLines)
                .ThenInclude(ol => ol.Product)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        // GET: Orders/Create
        public IActionResult Create()
        {
            var viewModel = new OrderWithOrderLineViewModel
            {
                Order = new Order(),
                OrderLines = new List<OrderLine> { new OrderLine() }
            };

            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId");

            var products = _context.Products
                .Select(p => new
                {
                    p.ProductId,
                    p.Price,
                    DisplayName = $"{p.Name} - €{p.Price:F2}"
                }).ToList();

            ViewData["ProductId"] = new SelectList(products, "ProductId", "DisplayName"); // geeft de namen van producten bij dropdown
            ViewBag.Products = products.ToDictionary(p => p.ProductId.ToString(), p => p.Price); // dictionary van ids product met prijzen

            return View(viewModel);
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderWithOrderLineViewModel viewModel)
        {
            // zorgt voor dat er een order en orderline is
            if (ModelState.IsValid && viewModel.OrderLines != null && viewModel.OrderLines.Any())
            {

                viewModel.Order.OrderDate = DateTime.Now;

                _context.Orders.Add(viewModel.Order);
                await _context.SaveChangesAsync(); 

                foreach (var orderLine in viewModel.OrderLines)
                {
                    orderLine.OrderId = viewModel.Order.OrderId; 
                    _context.OrderLines.Add(orderLine);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Confirmation), new { orderId = viewModel.Order.OrderId });

                
            }

            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", viewModel.Order.CustomerId);
            var products = _context.Products
        .Select(p => new
        {
            p.ProductId,
            DisplayName = $"{p.Name} - €{p.Price:F2}" 
        }).ToList();

            ViewData["ProductId"] = new SelectList(products, "ProductId", "DisplayName");

            return View(viewModel);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", order.CustomerId);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,OrderDate,CustomerId,Confirmed")] Order order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", order.CustomerId);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }

        public async Task<IActionResult> Confirmation(int? orderId)
        {
            if (orderId == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .FirstOrDefaultAsync(m => m.OrderId == orderId);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }
    }
}
