using LinkShortener.Models;
using LinkShortener.Services;
using MySql.Data.MySqlClient;
using Dapper;


var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddScoped<LinkShorteningService>();
var linkShorteningService = new LinkShorteningService();

var app = builder.Build();

app.UseHttpsRedirection();


app.MapGet("a/{skey}", (string skey) =>
{
    using var connection = new MySqlConnection("Server=localhost; User ID=root; Password=pass; Database=mydb");

    var lurl = connection.QueryFirstOrDefault<string>("select LongUrl from Links where ShortKey=@skey;", new { skey });

    if (lurl != null)
    {
        var uri = new Uri(lurl);
        return Results.Redirect(uri.AbsoluteUri);
    }

    return Results.BadRequest("URL not found.");

});


app.MapGet("/create/{*url}", (HttpContext context, string url) => {

    if (!Uri.TryCreate(url, UriKind.Absolute, out _))
    {
        return Results.BadRequest("URL is Invalid | Use full URL like https://addr.com");
    }

    using var connection = new MySqlConnection("Server=localhost; User ID=root; Password=pass; Database=mydb");

    var skey = connection.QueryFirstOrDefault<string>("select ShortKey from Links where LongUrl=@lurl;", new {lurl=url});

    var req = context.Request;
    var domainName = req.Scheme + "://" + req.Host.Value;

    if (skey != null)
    {
        return Results.Ok($"{domainName}/a/{skey}");
    }
    else
    {
        while (true)
        {
            var shortKey = linkShorteningService.GenerateShortLink(4);
            var q1 = "SELECT COUNT(*) FROM Links WHERE ShortKey = @shortKey";
            var count = connection.QueryFirstOrDefault<int>(q1, new { shortKey });

            if (count == 0)
            {
                var newLink = new Link() { LongUrl = url, ShortKey = shortKey, CreationTime = DateTime.Now };
                var affectedRows = connection.Execute("INSERT INTO Links (LongUrl, ShortKey, CreationTime) values (@LongUrl, @ShortKey, @CreationTime);", newLink);
                return Results.Ok($"{domainName}/a/{newLink.ShortKey}");
            }
        }

    }
});

app.Run();

