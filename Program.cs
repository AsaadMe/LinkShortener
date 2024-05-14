using LinkShortener.Models;
using LinkShortener.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

string connString = builder.Configuration.GetConnectionString("Mysqlcon");

builder.Services.AddTransient<LinkShorteningService>();
builder.Services.AddDbContext<LinkContext>(options =>
{
    options.UseMySQL(connString);
});

var app = builder.Build();

app.UseHttpsRedirection();


app.MapGet("a/{skey}", (string skey, LinkContext _context) =>
{
    using (_context)
    {
        var link = _context.Links.FirstOrDefault(l => l.ShortKey == skey);
        if (link != null)
        {
            var uri = new Uri(link.LongUrl);
            return Results.Redirect(uri.AbsoluteUri);
        }
    }

    return Results.BadRequest("URL not found.");

});


app.MapGet("/create/{*url}", (HttpContext context, string url, LinkShorteningService linkShorteningService, LinkContext _context) =>
{

    if (!Uri.TryCreate(url, UriKind.Absolute, out _))
    {
        return Results.BadRequest("URL is Invalid | Use full URL like https://addr.com");
    }

    var req = context.Request;
    var domainName = req.Scheme + "://" + req.Host.Value;

    using (_context)
    {
        _context.Database.EnsureCreated();
        var link = _context.Links.FirstOrDefault(l => l.LongUrl == url);
        if (link != null)
        {
            return Results.Ok($"{domainName}/a/{link.ShortKey}");
        }
        else
        {
            while (true)
            {
                var shortKey = linkShorteningService.GenerateShortLink(4);

                if (!_context.Links.Any(l => l.ShortKey == shortKey))
                {
                    var newLink = new Link() { LongUrl = url, ShortKey = shortKey, CreationTime = DateTime.Now };
                    _context.Links.Add(newLink);
                    _context.SaveChanges();
                    return Results.Ok($"{domainName}/a/{newLink.ShortKey}");
                }
            }
        }
    }
});

app.Run();