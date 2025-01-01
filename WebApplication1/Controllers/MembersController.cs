using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class MembersController : Controller
    {
        private readonly MVMemberContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public MembersController(MVMemberContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Members
        public async Task<IActionResult> Index(string searchString, string sortOrder)
        {
            // Sort
            ViewData["NameSort"] = sortOrder == "Name" ? "Name_desc" : "Name";
            ViewData["LastNameSort"] = sortOrder == "LastName" ? "LastName_desc" : "LastName";
            ViewData["EmailSort"] = sortOrder == "Email" ? "Email_desc" : "Email";
            ViewData["PhoneSort"] = sortOrder == "Phone" ? "Phone_desc" : "Phone";
            ViewData["AddressSort"] = sortOrder == "Address" ? "Address_desc" : "Address";
            ViewData["AmountSort"] = sortOrder == "Amount" ? "Amount_desc" : "Amount";


            // Get Members
            var members = from m in _context.Member
                          select m;


            if (!string.IsNullOrEmpty(searchString))
            {
                bool isNumber = double.TryParse(searchString, out double searchAmount);

                members = members.Where(m =>
                    m.Name.ToLower().Contains(searchString.ToLower()) ||
                    m.LastName.ToLower().Contains(searchString.ToLower()) ||
                    m.Email.ToLower().Contains(searchString.ToLower()) ||
                    m.Address.ToLower().Contains(searchString.ToLower()) ||
                    m.Phone.Contains(searchString) ||
                    isNumber && m.Amount.HasValue && m.Amount.Value == searchAmount
                );
            }

            // Sort
            members = sortOrder switch
            {
                "Name_desc" => members.OrderByDescending(m => m.Name),
                "LastName_desc" => members.OrderByDescending(m => m.LastName),
                "Email_desc" => members.OrderByDescending(n => n.Email),
                "Phone_desc" => members.OrderByDescending(n => n.Phone),
                "Address_desc" => members.OrderByDescending(n => n.Address),
                "Amount_desc" => members.OrderByDescending(n => n.Amount),

                "Name" => members.OrderBy(m => m.Name),
                "LastName" => members.OrderBy(m => m.LastName),
                "Email" => members.OrderBy(n => n.Email),
                "Phone" => members.OrderBy(n => n.Phone),
                "Address" => members.OrderBy(n => n.Address),
                "Amount" => members.OrderBy(n => n.Amount),

                _ => members.OrderBy(m => m.Name),
            };

            return View(await members.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Members
                .FirstOrDefaultAsync(m => m.Id == id);
            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        // GET: Members/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Members/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,LastName,Email,Phone,Address,City,Date,Amount")] Member member)
        {
            if (ModelState.IsValid)
            {
                _context.Add(member);
                await _context.SaveChangesAsync();

                // Ako je Amount == 25.00, dodijelite rolu "Member"
                if (member.Amount == 25.00)
                {
                    var user = await _userManager.FindByEmailAsync(member.Email);
                    if (user != null && !await _userManager.IsInRoleAsync(user, "Member"))
                    {
                        await _userManager.AddToRoleAsync(user, "Member");
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            return View(member);
        }

        // GET: Members/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Member.FindAsync(id);
            if (member == null)
            {
                return NotFound();
            }
            return View(member);
        }

        // POST: Members/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,LastName,Email,Phone,Address,City,Date,Amount")] Member member)
        {
            if (id != member.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(member);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MemberExists(member.Id))
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
            return View(member);
        }

        // GET: Members/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Member
                .FirstOrDefaultAsync(m => m.Id == id);
            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        // POST: Members/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var member = await _context.Member.FindAsync(id);
            if (member != null)
            {
                _context.Member.Remove(member);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MemberExists(int id)
        {
            return _context.Member.Any(e => e.Id == id);
        }
    }
}
