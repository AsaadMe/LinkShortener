using LinkShortener.Models;
using LinkShortener.Services;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddScoped<LinkShorteningService>();
var linkShorteningService = new LinkShorteningService();

var app = builder.Build();

app.UseHttpsRedirection();


app.MapGet("a/{skey}", (string skey) =>
{
    using (var dbcontext = new LinkContext())
    {
        var link = dbcontext.Links.FirstOrDefault(l => l.ShortKey == skey);
        if (link != null)
        {
            var uri = new Uri(link.LongUrl);
            return Results.Redirect(uri.AbsoluteUri);
        }
    }

    return Results.BadRequest("URL not found.");

});


app.MapGet("/create/{*url}", (HttpContext context, string url) =>
{

    if (!Uri.TryCreate(url, UriKind.Absolute, out _))
    {
        return Results.BadRequest("URL is Invalid | Use full URL like https://addr.com");
    }

    var req = context.Request;
    var domainName = req.Scheme + "://" + req.Host.Value;

    using (var dbcontext = new LinkContext())
    {
        var link = dbcontext.Links.FirstOrDefault(l => l.LongUrl == url);
        if (link != null)
        {
            return Results.Ok($"{domainName}/a/{link.ShortKey}");
        }
        else
        {
            while (true)
            {
                var shortKey = linkShorteningService.GenerateShortLink(4);

                if (dbcontext.Links.FirstOrDefault(l => l.ShortKey == shortKey) == null)
                {
                    var newLink = new Link() { LongUrl = url, ShortKey = shortKey, CreationTime = DateTime.Now };
                    InsertData(newLink);
                    return Results.Ok($"{domainName}/a/{newLink.ShortKey}");
                }
            }
        }
    }
});

app.Run();

static void InsertData(Link link)
{
    using (var dbcontext = new LinkContext())
    {
        // Creates the database if not exists
        dbcontext.Database.EnsureCreated();

        // Adds some books
        dbcontext.Links.Add(new Link
        {
            LongUrl = link.LongUrl,
            ShortKey = link.ShortKey,
            CreationTime = link.CreationTime
        });

        // Saves changes
        dbcontext.SaveChanges();
    }
}