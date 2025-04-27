using Microsoft.AspNetCore.Mvc;
using Coursework_AMD.Models;
using Coursework_AMD.Data;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics;

namespace Coursework_AMD.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ApplicationDbContext context, UserManager<IdentityUser> userManager, ILogger<HomeController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<UrlMapping> urls;

            if ((User?.Identity != null && User.Identity.IsAuthenticated))  
            {
                var userId = _userManager.GetUserId(User);
                urls = _context.UrlMappings
                    .Where(u => u.UserId == userId)
                    .OrderByDescending(u => u.CreatedAt)
                    .Take(10)
                    .ToList();
            }

            else { 
                urls = new List<UrlMapping>(); // Or show public URLs if preferred
            }

            ViewBag.RecentUrls = urls;
            return View();
        }

        // POST: /Home/Index
        // This action handles the form submission for URL shortening
        [HttpPost]
        public async Task<IActionResult> Index(string originalUrl)
        {
            if (!Uri.IsWellFormedUriString(originalUrl, UriKind.Absolute))
            {
                ModelState.AddModelError("", "Invalid URL format");
                return View();
            }

            string shortCode = GenerateShortCode();
            string? userId = (User?.Identity != null && User.Identity.IsAuthenticated) ? _userManager.GetUserId(User) : null; // Get user ID if authenticated

            var mapping = new UrlMapping
            {
                OriginalUrl = originalUrl,
                ShortCode = shortCode,
                UserId = userId
            };

            _context.UrlMappings.Add(mapping);
            await _context.SaveChangesAsync();

            ViewBag.ShortUrl = Url.Action("RedirectTo", "Url", new { code = shortCode }, Request.Scheme);

            // Refresh filtered List
            var urls = userId != null
                ? _context.UrlMappings
                    .Where(u => u.UserId == userId)
                    .OrderByDescending(u => u.CreatedAt)
                    .Take(10)
                    .ToList()
                    : new List<UrlMapping>();

            ViewBag.RecentUrls = urls;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // Generate a random short code
        private string GenerateShortCode(int length = 6)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}