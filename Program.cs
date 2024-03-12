using LinkShortener.Models;
using LinkShortener.Services;
using MySql.Data;
using MySql.Data.MySqlClient;
using Dapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Policy;


var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddScoped<LinkShorteningService>();
var linkShorteningService = new LinkShorteningService();

var app = builder.Build();

app.UseHttpsRedirection();


app.MapGet("a/{code}", (string code) =>
{
    using var connection = new MySqlConnection("Server=localhost; User ID=root; Password=pass; Database=mydb");

    var lurl = connection.QueryFirstOrDefault<string>("select LongUrl from Links where ShortUrl=@surl;", new { surl = code });

    if (lurl != null)
    {
        return Results.Redirect(lurl);
    }

    return Results.BadRequest("URL not found.");

});


app.MapGet("/create/{*url}", (string url) => {

    if (!Uri.TryCreate(url, UriKind.Absolute, out _))
    {
        return Results.BadRequest("URL is Invalid | Use full URL like https://addr.com");
    }

    using var connection = new MySqlConnection("Server=localhost; User ID=root; Password=pass; Database=mydb");

    var surl = connection.QueryFirstOrDefault<string>("select ShortUrl from Links where LongUrl=@lurl;", new {lurl=url});

    if (surl != null)
    {
        return Results.Ok($"Short URL: {surl}");
    }
    else
    {
        while (true)
        {
            var shortKey = linkShorteningService.GenerateShortLink(4);
            var q1 = "SELECT COUNT(*) FROM Links WHERE ShortUrl = @shortKey";
            var count = connection.QueryFirstOrDefault<int>(q1, new { shortKey = shortKey });

            if (count == 0)
            {
                var newLink = new Link() { LongUrl = url, ShortUrl = shortKey, CreationTime = DateTime.Now };
                var affectedRows = connection.Execute("INSERT INTO Links values (@LongUrl, @ShortUrl, @CreationTime);", newLink);
                return Results.Ok($"Short URL: https://localhost:7168/a/{newLink.ShortUrl}");
            }
        }

    }
});

app.Run();

