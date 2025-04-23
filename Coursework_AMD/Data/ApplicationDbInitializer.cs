using Coursework_AMD.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Coursework_AMD.Data
{
    public static class ApplicationDbInitializer
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            // Ensure DB is created
            await context.Database.MigrateAsync();

            // Only seed if there are no URLs
            if (!context.UrlMappings.Any())
            {
                var sampleUrls = new List<UrlMapping>
                {
                    new UrlMapping
                    {
                        OriginalUrl = "https://www.google.com",
                        ShortCode = "googl1"
                    },
                    new UrlMapping
                    {
                        OriginalUrl = "https://www.microsoft.com",
                        ShortCode = "msft01"
                    },
                    new UrlMapping
                    {
                        OriginalUrl = "https://dotnet.microsoft.com",
                        ShortCode = "dotnet"
                    }
                };

                context.UrlMappings.AddRange(sampleUrls);
                await context.SaveChangesAsync();
            }
        }
    }
}
