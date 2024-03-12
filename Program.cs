using LinkShortener.Models;
using LinkShortener.Services;
using MySql.Data;
using MySql.Data.MySqlClient;
using Dapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;


var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddScoped<LinkShorteningService>();
var linkShorteningService = new LinkShorteningService();

// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

//app.MapPost("/", (LinkShorteningService linkShorteningService, HttpContext httpContext) =>
//{


//    using var connection = new MySqlConnection("Server=localhost; User ID=root; Password=pass; Database=mydb");

//    var link = connection.Query<Link>("select * from testtable;").ToList();

//    return link;
//});

app.MapGet("a/{code}", (string code) =>
{
    return Results.Redirect("https://google.com");
    //return Results.Ok($"{code}");

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
                var newLink = new Link() { LongUrl = url, ShortUrl = $"https://avi.com/a/{shortKey}", CreationTime = DateTime.Now };
                var affectedRows = connection.Execute("INSERT INTO Links values (@LongUrl, @ShortUrl, @CreationTime);", newLink);
                return Results.Ok($"Short URL: {newLink.ShortUrl}");
            }
        }

    }
});

app.Run();

