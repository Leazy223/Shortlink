using Microsoft.AspNetCore.Mvc;
using Coursework_AMD.Data;

namespace Coursework_AMD.Controllers
{
    public class UrlController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UrlController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("/{code}")]
        public IActionResult RedirectTo(string code) 
        {
            var mapping = _context.UrlMappings.FirstOrDefault(u => u.ShortCode == code);
            if (mapping == null)
                return NotFound();

            return Redirect(mapping.OriginalUrl);
        }

        public IActionResult History()
        {
            var userId = (User?.Identity != null && User.Identity.IsAuthenticated) ? User.Identity.Name : null;
            if (userId == null) return RedirectToAction("Login", "Account");

            var urls = _context.UrlMappings.Where(u => u.UserId == userId).ToList();
            return View(urls);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!(User?.Identity != null && User.Identity.IsAuthenticated))
                return Unauthorized();

            var userId = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var mapping = _context.UrlMappings.FirstOrDefault(u => u.Id == id && u.UserId == userId);

            if (mapping == null)
                return NotFound();

            _context.UrlMappings.Remove(mapping);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        // GET: /Url/Edit/5
        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (!(User?.Identity != null && User.Identity.IsAuthenticated))
                return Unauthorized();

            var userId = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var mapping = _context.UrlMappings.FirstOrDefault(u => u.Id == id && u.UserId == userId);

            if (mapping == null)
                return NotFound();

            return View(mapping);
        }

        // POST: /Url/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string originalUrl, string shortCode)
        {
            if (!(User?.Identity != null && User.Identity.IsAuthenticated))
                return Unauthorized();

            var userId = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var mapping = _context.UrlMappings.FirstOrDefault(u => u.Id == id && u.UserId == userId);

            if (mapping == null)
                return NotFound();

            if (!Uri.IsWellFormedUriString(originalUrl, UriKind.Absolute))
            {
                ModelState.AddModelError("", "Invalid URL format.");
                return View(mapping);
            }

            // Optional: Check if short code already exists (excluding current one)
            bool shortCodeExists = _context.UrlMappings
                .Any(u => u.ShortCode == shortCode && u.Id != id);

            if (shortCodeExists)
            {
                ModelState.AddModelError("", "Short code already in use. Please choose another.");
                return View(mapping);
            }

            mapping.OriginalUrl = originalUrl;
            mapping.ShortCode = shortCode;
            _context.Update(mapping);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }
    }
}