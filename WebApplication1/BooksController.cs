using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using static System.Reflection.Metadata.BlobBuilder;

namespace WebApplication1
{
    public class BooksController : Controller
    {
        private readonly MVCLibraryContext _context;

        public BooksController(MVCLibraryContext context)
        {
            _context = context;
        }
        // GET: Books/Index
        public async Task<IActionResult> Index(string searchString, string sortOrder)
        {
            // Sort
            ViewData["TitleSortParm"] = sortOrder == "Title" ? "Title_desc" : "Title";
            ViewData["AuthorSortParm"] = sortOrder == "Author" ? "Author_desc" : "Author";
            ViewData["GenreSortParm"] = sortOrder == "Genre" ? "Genre_desc" : "Genre";
            ViewData["QuantitySortParm"] = sortOrder == "Quantity" ? "Quantity_desc" : "Quantity";
            ViewData["AvailabilitySortParm"] = sortOrder == "Availability" ? "Availability_desc" : "Availability";

            // Get Books
            var books = from b in _context.Book
                        select b;

            // Search
            if (!string.IsNullOrEmpty(searchString))
            {
                books = books.Where(b => b.Title.ToLower().Contains(searchString.ToLower()) ||
                                          b.Author.ToLower().Contains(searchString.ToLower()) ||
                                          b.Genre.ToLower().Contains(searchString.ToLower()));
            }

            // Sort
            books = sortOrder switch
            {
                "Title_desc" => books.OrderByDescending(b => b.Title),
                "Author_desc" => books.OrderByDescending(b => b.Author),
                "Genre_desc" => books.OrderByDescending(b => b.Genre),
                "Quantity_desc" => books.OrderByDescending(b => b.Quantity),
                "Availability_desc" => books.OrderByDescending(b => b.Availability),
                "Title" => books.OrderBy(b => b.Title),
                "Author" => books.OrderBy(b => b.Author),
                "Genre" => books.OrderBy(b => b.Genre),
                "Quantity" => books.OrderBy(b => b.Quantity),
                "Availability" => books.OrderBy(b => b.Availability),
                _ => books.OrderBy(b => b.Title),
            };

            return View(await books.ToListAsync());
        }



        // GET: Books
        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .Include(b => b.Reviews) // Uključujemo recenzije
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/AddReview
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(int bookId, string userName, string comment, int rating)
        {
            if (rating < 1 || rating > 5)
            {
                ModelState.AddModelError("Rating", "Rating must be between 1 and 5.");
                return RedirectToAction("Details", new { id = bookId });
            }

            var review = new Review
            {
                BookId = bookId,
                UserName = userName,
                Comment = comment,
                Rating = rating
            };

            _context.Add(review);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = bookId });
        }

        

        // GET: Books/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Author,Genre,Availability,Quantity")] Book book)
        {
            if (ModelState.IsValid)
            {
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Author,Genre,Availability,Quantity")] Book book)
        {
            if (id != book.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id))
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
            return View(book);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Book.FindAsync(id);
            if (book != null)
            {
                _context.Book.Remove(book);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return _context.Book.Any(e => e.Id == id);
        }
    }
}
