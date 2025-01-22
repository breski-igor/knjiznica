using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using System.Security.Claims;
using WebApplication1.Data;

namespace WebApplication1.Controllers
{
    [Authorize(Roles = "Member, Admin")]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Orders


        public async Task<IActionResult> Index(string searchString, string sortOrder)
        {
            // Sortiranje
            ViewData["FirstNameSort"] = sortOrder == "FirstName" ? "FirstName_desc" : "FirstName";
            ViewData["LastNameSort"] = sortOrder == "LastName" ? "LastName_desc" : "LastName";
            ViewData["EmailSort"] = sortOrder == "Email" ? "Email_desc" : "Email";
            ViewData["AdressSort"] = sortOrder == "Adress" ? "Adress_desc" : "Adress";
            ViewData["BookNameSort"] = sortOrder == "BookName" ? "BookName_desc" : "BookName";
            ViewData["WriterNameSort"] = sortOrder == "WriterName" ? "WriterName_desc" : "WriterName";
            ViewData["OrderDateSort"] = sortOrder == "OrderDate" ? "OrderDate_desc" : "OrderDate";
            ViewData["DateSentSort"] = sortOrder == "DateSent" ? "DateSent_desc" : "DateSent";
            ViewData["DateReturnedSort"] = sortOrder == "DateReturned" ? "DateReturned_desc" : "DateReturned";

            // Dohvati narudžbe
            var orders = from o in _context.Order
                         select o;

            if (!string.IsNullOrEmpty(searchString))
            {
                // Provjerava ako je broj za Amount (ako želite pretraživati količinu ili slične vrijednosti)
                bool isNumber = double.TryParse(searchString, out double searchAmount);

                orders = orders.Where(o =>
                    o.First_Name.ToLower().Contains(searchString.ToLower()) ||
                    o.Last_Name.ToLower().Contains(searchString.ToLower()) ||
                    o.Email.ToLower().Contains(searchString.ToLower()) ||
                    o.Adress.ToLower().Contains(searchString.ToLower()) ||
                    o.Book_Name.ToLower().Contains(searchString.ToLower()) ||
                    o.Writer_Name.ToLower().Contains(searchString.ToLower()) ||
                    o.Order_Date != null && o.Order_Date.ToString().Contains(searchString) ||
                    o.Date_Sent != null && o.Date_Sent.ToString().Contains(searchString) ||
                    o.Date_Returned != null && o.Date_Returned.ToString().Contains(searchString)
                );
            }

            // Sortiranje
            orders = sortOrder switch
            {
                "FirstName_desc" => orders.OrderByDescending(o => o.First_Name),
                "LastName_desc" => orders.OrderByDescending(o => o.Last_Name),
                "Email_desc" => orders.OrderByDescending(o => o.Email),
                "Adress_desc" => orders.OrderByDescending(o => o.Adress),
                "BookName_desc" => orders.OrderByDescending(o => o.Book_Name),
                "WriterName_desc" => orders.OrderByDescending(o => o.Writer_Name),
                "OrderDate_desc" => orders.OrderByDescending(o => o.Order_Date),
                "DateSent_desc" => orders.OrderByDescending(o => o.Date_Sent),
                "DateReturned_desc" => orders.OrderByDescending(o => o.Date_Returned),

                "FirstName" => orders.OrderBy(o => o.First_Name),
                "LastName" => orders.OrderBy(o => o.Last_Name),
                "Email" => orders.OrderBy(o => o.Email),
                "Adress" => orders.OrderBy(o => o.Adress),
                "BookName" => orders.OrderBy(o => o.Book_Name),
                "WriterName" => orders.OrderBy(o => o.Writer_Name),
                "OrderDate" => orders.OrderBy(o => o.Order_Date),
                "DateSent" => orders.OrderBy(o => o.Date_Sent),
                "DateReturned" => orders.OrderBy(o => o.Date_Returned),

                _ => orders.OrderBy(o => o.First_Name),
            };
            foreach (var order in orders)
            {
                DateTime? dateSent = order.Date_Sent?.ToDateTime(TimeOnly.MinValue);

                if (dateSent.HasValue)
                {
                    TimeSpan duration = DateTime.Now - dateSent.Value;

                    if (duration.Days > 30)
                    {
                        order.IsDateSentRed = true;
                    }
                    else
                    {
                        order.IsDateSentRed = false;
                    }

                }
            }
            return View(await orders.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }


        // GET: Orders/Create

        

[Authorize]
    public ActionResult Create(string bookName, string writerName)
    {
        var email = User.Identity.IsAuthenticated
            ? User.FindFirst(ClaimTypes.Email)?.Value
            : null;

        var order = new Order
        {
            Email = email, // Automatski postavite email u model
            Book_Name = bookName,
            Writer_Name = writerName
        };

        return View(order);
    }


    // POST: Orders/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create([Bind("Id,First_Name,Last_Name,Email,Adress,Book_Name,Writer_Name")] Order order)
        {
            if (ModelState.IsValid)
            {
                order.Order_Date = DateOnly.FromDateTime(DateTime.Now);
                order.Date_Sent = null;
                order.Date_Returned = null;

                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,First_Name,Last_Name,Email,Adress,Book_Name,Writer_Name,Order_Date,Date_Sent,Date_Returned")] Order order)
        {
            if (id != order.Id)
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
                    if (!OrderExists(order.Id))
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
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .FirstOrDefaultAsync(m => m.Id == id);
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
            var order = await _context.Order.FindAsync(id);
            if (order != null)
            {
                _context.Order.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Order.Any(e => e.Id == id);
        }
    }
}
