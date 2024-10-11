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
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace FrituurAllRelationships.Controllers
{
    // deze pagina is nog onder constructie
    public class CustomersController : Controller
    {
        private readonly FrituurDb _context;

        public CustomersController(FrituurDb context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }


            var customer = _context.Customers.FirstOrDefault(c => c.Username == model.Username && c.Password == model.Password);

            if (customer != null)
            {
                // claims worden de ingelogde gegevens van de klant in opgeslagen (naam en id)
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, customer.Username),
            new Claim(ClaimTypes.NameIdentifier, customer.CustomerId.ToString())
        };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // Claims worden opgeslagen met cookies
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Invalid username or password.");
            return View(model);
        }

        // Wordt niet toegepast
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            //login cookies worden vergeten
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerId,Username,Password")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Login));
            }
            return View(customer);
        }
    }

}

